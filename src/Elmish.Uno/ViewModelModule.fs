module Elmish.Uno.ViewModel

/// Creates a design-time view model using the given model and bindings.
[<CompiledName "DesignInstance">]
let designInstance (model: 'model) (bindings: Binding<'model, 'msg> list) =
  let args = ViewModelArgs.simple model

  DynamicViewModel(args, bindings) |> box
