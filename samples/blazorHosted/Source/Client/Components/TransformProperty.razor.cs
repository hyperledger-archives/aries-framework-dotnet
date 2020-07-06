namespace Hyperledger.Aries.AspNetCore.Components
{
  using Microsoft.AspNetCore.Components;
  using System;
  public partial class TransformProperty<TTransformed, TSource>
  {
    [Parameter] 
    public Func<TTransformed> GetTransformed { get; set; }


    [Parameter] 
    public Action<TTransformed> SetSource { get; set; }


    [Parameter] 
    public RenderFragment<TransformProperty<TTransformed, TSource>> ChildContent { get; set; }

    [Parameter] public EventCallback<TTransformed> TransformedValueChanged { get; set; }

    public TTransformed TransformedValue
    {
      get => GetTransformed();
      set
      {
        SetSource(value);
        TransformedValueChanged.InvokeAsync(value);
      }
    }
  }
}
