namespace WJb.Demo.Wasm;

public static class EmbeddedConfig
{
    public const string ActionsJson = """
    {
      "hello": {
        "type": "WJb.Demo.Wasm.Actions.HelloAction, WJb.Demo.Wasm"
      },
      "configured": {
        "type": "WJb.Demo.Wasm.Actions.ConfiguredAction, WJb.Demo.Wasm"
      }
    """;

    public const string ServicesJson = """
    {
      "smtp": {
        "type": "SmtpSettings",
        "host": "smtp.local"
      }
    }
    """;
}


//  "progress": {
//    "type": "WJb.Demo.Wasm.Actions.ProgressAction, WJb.Demo.Wasm"
//  },



//  "send-email": {
//"type": "WJb.Demo.Wasm.Actions.SendEmailAction, WJb.Demo.Wasm",
//    "smtpCode": "smtp"
//  },

//  "log": {
//"type": "WJb.Demo.Wasm.Actions.LogAction, WJb.Demo.Wasm"
//  },

//  "error-log": {
//"type": "WJb.Demo.Wasm.Actions.ErrorLogAction, WJb.Demo.Wasm"
//  },

//  "retry-email": {
//"type": "WJb.Demo.Wasm.Actions.RetryEmailAction, WJb.Demo.Wasm"
//  },

//  "create-order": {
//"type": "WJb.Demo.Wasm.Actions.CreateOrderAction, WJb.Demo.Wasm"
//  },

//  "reserve-stock": {
//"type": "WJb.Demo.Wasm.Actions.ReserveStockAction, WJb.Demo.Wasm"
//  },

//  "charge-payment": {
//"type": "WJb.Demo.Wasm.Actions.ChargePaymentAction, WJb.Demo.Wasm"
//  },

//  "send-confirmation": {
//"type": "WJb.Demo.Wasm.Actions.SendConfirmationAction, WJb.Demo.Wasm"
//  },

//  "demo": {
//"type": "DemoAction, WJb.Demo.Wasm"
//  }
