namespace Hyperledger.Aries.AspNetCore.Components.Forms
{
  using Microsoft.AspNetCore.Components.Forms;

  public class InputSelectNumber<T> : InputSelect<T>
  {
    protected override bool TryParseValueFromString(string aValue, out T aResult, out string aValidationErrorMessage)
    {
      if (typeof(T) == typeof(int))
      {
        if (int.TryParse(aValue, out int resultInt))
        {
          aResult = (T)(object)resultInt;
          aValidationErrorMessage = null;
          return true;
        }
        else
        {
          aResult = default;
          aValidationErrorMessage = "The chosen value is not a valid number.";
          return false;
        }
      }
      else
      {
        return base.TryParseValueFromString(aValue, out aResult, out aValidationErrorMessage);
      }
    }
  }
}
