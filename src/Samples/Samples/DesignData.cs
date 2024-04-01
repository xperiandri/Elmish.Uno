namespace Elmish.Uno.Samples;

#pragma warning disable CA1812 // ... is an internal class that is apparently never instantiated. If so, remove the code from the assembly. If this class is intended to contain only static members, make it static
#pragma warning disable CA1822 // Member does not access instance data and can be marked as static.

using System;
using System.Collections.Generic;
using System.Text;

#pragma warning disable CA1812 // Avoid uninstantiated internal classes
#pragma warning disable CA1822 // Member does not access instance data and can be marked as static
using EventBindingsAndBehaviorsProgram = Elmish.Uno.Samples.EventBindingsAndBehaviors.Program;
using FileDialogsCmdMsgProgram = Elmish.Uno.Samples.FileDialogsCmdMsg.Program;
using FileDialogsProgram = Elmish.Uno.Samples.FileDialogs.Program;
using NewWindow1Program = Elmish.Uno.Samples.NewWindow.Program.Win1;
using NewWindow2Program = Elmish.Uno.Samples.NewWindow.Program.Win2;
using NewWindowProgram = Elmish.Uno.Samples.NewWindow.Program;
using OneWaySeqProgram = Elmish.Uno.Samples.OneWaySeq.Program;
using SingleCounterProgram = Elmish.Uno.Samples.SingleCounter.Program;
using SubModelClockProgram = Elmish.Uno.Samples.SubModel.Program.Clock;
using SubModelCounterWithClockProgram = Elmish.Uno.Samples.SubModel.Program.CounterWithClock;
using SubModelOptForm1Program = Elmish.Uno.Samples.SubModelOpt.Program.Form1;
using SubModelOptForm2Program = Elmish.Uno.Samples.SubModelOpt.Program.Form2;
using SubModelOptProgram = Elmish.Uno.Samples.SubModelOpt.Program;
using SubModelProgram = Elmish.Uno.Samples.SubModel.Program;
using SubModelSelectedItemProgram = Elmish.Uno.Samples.SubModelSelectedItem.Program;
using SubModelSeqProgram = Elmish.Uno.Samples.SubModelSeq.Program;
using UiBoundCmdParamProgram = Elmish.Uno.Samples.UiBoundCmdParam.Program;
using ValidationProgram = Elmish.Uno.Samples.Validation.Program;

internal class DesignData
{
    public object SingleCounter => SingleCounterProgram.DesignInstance;
    public object OneWaySeq => OneWaySeqProgram.DesignInstance;
    public object SubModel => SubModelProgram.DesignInstance;
    public object SubModelClock => SubModelClockProgram.DesignInstance;
    public object SubModelCounterWithClock => SubModelCounterWithClockProgram.DesignInstance;
    public object SubModelOpt => SubModelOptProgram.DesignInstance;
    public object SubModelOptForm1 => SubModelOptForm1Program.DesignInstance;
    public object SubModelOptForm2 => SubModelOptForm2Program.DesignInstance;
    public object SubModelSelectedItem => SubModelSelectedItemProgram.DesignInstance;
    public object SubModelSeq => SubModelSeqProgram.DesignInstance;
    public object UiBoundCmdParam => UiBoundCmdParamProgram.DesignInstance;
    public object Validation => ValidationProgram.DesignInstance;
    public object FileDialogs => FileDialogsProgram.DesignInstance;
    public object FileDialogsCmdMsg => FileDialogsCmdMsgProgram.DesignInstance;
    public object EventBindingsAndBehaviors => EventBindingsAndBehaviorsProgram.DesignInstance;
    public object NewWindow => NewWindowProgram.DesignInstance;
    public object NewWindow1 => NewWindow1Program.DesignInstance;
    public object NewWindow2 => NewWindow2Program.DesignInstance;
}
