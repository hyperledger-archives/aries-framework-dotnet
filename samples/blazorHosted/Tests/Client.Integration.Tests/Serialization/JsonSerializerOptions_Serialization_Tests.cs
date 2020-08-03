namespace JsonSerializerOptions
{
  using FluentAssertions;
  using System;
  using System.Text.Json;
  using Hyperledger.Aries.AspNetCore.JsonSerializer.Tests;

  public class JsonSerializer_Should
  {
    public void SerializeAndDeserializePerson()
    {
      var jsonSerializerOptions = new JsonSerializerOptions();
      var person = new Person { FirstName = "Steve", LastName = "Cramer", BirthDay = new DateTime(1967, 09, 27) };
      string json = JsonSerializer.Serialize(person, jsonSerializerOptions);
      Person parsed = JsonSerializer.Deserialize<Person>(json, jsonSerializerOptions);
      parsed.BirthDay.Should().Be(person.BirthDay);
      parsed.FirstName.Should().Be(person.FirstName);
      parsed.LastName.Should().Be(person.LastName);
    }
  }
}
