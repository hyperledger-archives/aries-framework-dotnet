const dispatchIncrementCountAction = () => {
  console.log("%cdispatchIncrementCountAction", "color: green");
  const IncrementCountActionName = "blazorhosted.Features.Counters.CounterState+IncrementCounterAction, blazorhosted.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
  const blazorState = window["BlazorState"];
  blazorState.DispatchRequest(IncrementCountActionName, { amount: 7 });
};

function initialize() {
  console.log("Initialize InteropTest.js");
  window["InteropTest"] = dispatchIncrementCountAction;
}

initialize();