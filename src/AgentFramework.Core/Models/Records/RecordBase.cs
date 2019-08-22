using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace AgentFramework.Core.Models.Records
{
    /// <summary>
    /// Wallet record.
    /// </summary>
    public abstract class RecordBase
    {
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <returns>The identifier.</returns>
        public virtual string Id { get; set; }

        /// <summary>
        /// Gets the created at datetime of the record.
        /// </summary>
        /// <returns>The created datetime of the record.</returns>
        [JsonIgnore]
        public DateTime? CreatedAtUtc
        {
            get => GetDateTime();
            internal set => Set(value, false);
        }
        
        /// <summary>
        /// Gets the last updated datetime of the record.
        /// </summary>
        /// <returns>The last updated datetime of the record.</returns>
        [JsonIgnore]
        public DateTime? UpdatedAtUtc
        {
            get => GetDateTime();
            internal set => Set(value, false);
        }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        /// <returns>The type name.</returns>
        [JsonIgnore]
        public abstract string TypeName { get; }

        /// <summary>Gets or sets the tags.</summary>
        /// <value>The tags.</value>
        [JsonIgnore]
        protected internal Dictionary<string, string> Tags { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <param name="name">Name.</param>
        public string GetTag(string name) => Get(name: name);

        /// <summary>
        /// Sets the attribute.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="value">Value.</param>
        /// <param name="encrypted">Controls if the tag is encrypted.</param>
        public void SetTag(string name, string value, bool encrypted = true) => Set(value, encrypted, name);

        /// <summary>
        /// Removes a user attribute.
        /// </summary>
        /// <returns>The attribute.</returns>
        /// <param name="name">Name.</param>
        public void RemoveTag(string name) => Set(null, name: name);

        /// <summary>
        /// Set the specified value, field and name.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="encrypted">Controls whether the stored attribute should be encrypted at rest</param>
        /// <param name="name">Name.</param>
        protected void Set(string value, bool encrypted = true, [CallerMemberName]string name = "")
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name), "Attribute name must be specified.");

            if (!encrypted)
                name = $"~{name}";

            if (value != null)
            {
                Tags[name] = value;
            }
            else if (Tags.ContainsKey(name))
            {
                Tags.Remove(name);
            }
        }

        /// <summary>
        /// Set the specified value, field and name.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="field">Field.</param>
        /// <param name="encrypted">Controls whether the stored attribute should be encrypted at rest</param>
        /// <param name="name">Name.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        protected void Set<T>(T value, ref T field, bool encrypted = true, [CallerMemberName]string name = "") where T : struct
        {
            Set(value, encrypted, name);
            field = value;
        }

        /// <summary>
        /// Set the specified value, field and name.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="field">Field.</param>
        /// <param name="encrypted">Controls whether the stored attribute should be encrypted at rest</param>
        /// <param name="name">Name.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        protected void Set<T>(T? value, ref T? field, bool encrypted = true, [CallerMemberName]string name = "") where T : struct
        {
            Set(value, encrypted, name);
            field = value;
        }

        /// <summary>
        /// Set the specified value, field and name.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="encrypted">Controls whether the stored attribute should be encrypted at rest</param>
        /// <param name="name">Name.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        protected void Set<T>(T? value, bool encrypted = true, [CallerMemberName]string name = "") where T : struct
        {
            if (typeof(T) == typeof(DateTime))
            {
                var dateVal = value as DateTime?;

                if (dateVal != null)
                {
                    var strVal = dateVal.Value.Ticks.ToString();
                    Set(strVal, name: name, encrypted: encrypted);
                }
                else
                    Set(null, name: name, encrypted: encrypted);
            }
            else if (typeof(T).IsEnum)
            {
                var enumVal = (value as Enum);

                if (enumVal != null)
                    Set((value as Enum).ToString("G"), name: name, encrypted: encrypted);
                else
                    Set(null, name: name, encrypted: encrypted);
            }
            else
                Set(value.ToString(), name: name, encrypted: encrypted);
        }

        /// <summary>
        /// Set the specified value, field and name.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="encrypted">Controls whether the stored attribute should be encrypted at rest</param>
        /// <param name="name">Name.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        protected void Set<T>(T value, bool encrypted = true, [CallerMemberName]string name = "") where T : struct
        {
            if (typeof(T) == typeof(DateTime))
            {
                var dateVal = value as DateTime?;

                if (dateVal != null)
                {
                    var strVal = ((DateTimeOffset)dateVal.Value).ToUnixTimeMilliseconds().ToString();
                    Set(strVal, name: name, encrypted: encrypted);
                }
                else
                    Set(null, name: name, encrypted: encrypted);
            }
            else if (typeof(T).IsEnum)
            {
                var enumVal = (value as Enum);

                if (enumVal != null)
                    Set((value as Enum).ToString("G"), name: name, encrypted: encrypted);
                else
                    Set(null, name: name, encrypted: encrypted);
            }
            else
                Set(value.ToString(), name: name, encrypted: encrypted);
        }

        /// <summary>
        /// Get the value of the specified tag name.
        /// </summary>
        /// <returns>The get.</returns>
        /// <param name="name">Name.</param>
        protected string Get([CallerMemberName]string name = "")
        {
            if (Tags.ContainsKey(name))
                return Tags[name];
            if (Tags.ContainsKey($"~{name}"))
                return Tags[$"~{name}"];
            return null;
        }

        /// <summary>Gets the date time.</summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        protected DateTime? GetDateTime([CallerMemberName] string name = "")
        {
            var strVal = Get(name);

            if (strVal == null)
                return null;

            return new DateTime(Convert.ToInt64(strVal));
        }

        /// <summary>Gets the bool.</summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        protected bool GetBool([CallerMemberName] string name = "")
        {
            var strVal = Get(name);

            if (strVal == null)
                return false;

            return Convert.ToBoolean(strVal);
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().Name}: " +
            $"Id={Id}, " +
            $"TypeName={TypeName}, " +
            $"CreatedAtUtc={CreatedAtUtc}, " +
            $"UpdatedAtUtc={UpdatedAtUtc}";
    }
}
