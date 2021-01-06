using System;
using System.Collections.Generic;
using System.Text;

using SingleCounterProgram = Elmish.Uno.Samples.SingleCounter.Program;
using OneWaySeqProgram = Elmish.Uno.Samples.OneWaySeq.Program;
using SubModelProgram = Elmish.Uno.Samples.SubModel.Program;
using SubModelClockProgram = Elmish.Uno.Samples.SubModel.Program.Clock;
using SubModelCounterWithClockProgram = Elmish.Uno.Samples.SubModel.Program.CounterWithClock;
using SubModelOptProgram = Elmish.Uno.Samples.SubModelOpt.Program;
using SubModelOptForm1Program = Elmish.Uno.Samples.SubModelOpt.Program.Form1;
using SubModelOptForm2Program = Elmish.Uno.Samples.SubModelOpt.Program.Form2;
using SubModelSelectedItemProgram = Elmish.Uno.Samples.SubModelSelectedItem.Program;
using SubModelSeqProgram = Elmish.Uno.Samples.SubModelSeq.Program;
using UiBoundCmdParamProgram = Elmish.Uno.Samples.UiBoundCmdParam.Program;
using ValidationProgram = Elmish.Uno.Samples.Validation.Program;
using FileDialogsProgram = Elmish.Uno.Samples.FileDialogs.Program;
using FileDialogsCmdMsgProgram = Elmish.Uno.Samples.FileDialogsCmdMsg.Program;
using EventBindingsAndBehaviorsProgram = Elmish.Uno.Samples.EventBindingsAndBehaviors.Program;
using NewWindowProgram = Elmish.Uno.Samples.NewWindow.Program;
using NewWindow1Program = Elmish.Uno.Samples.NewWindow.Program.Win1;
using NewWindow2Program = Elmish.Uno.Samples.NewWindow.Program.Win2;

namespace Elmish.Uno.Samples
{
    internal class DesignData
    {
        public object SingleCounter => ViewModel.DesignInstance(SingleCounterProgram.DesignModel, SingleCounterProgram.Program);
        public object OneWaySeq => ViewModel.DesignInstance(OneWaySeqProgram.DesignModel, OneWaySeqProgram.Program);
        public object SubModel => ViewModel.DesignInstance(SubModelProgram.DesignModel, SubModelProgram.Program);
        public object SubModelClock => ViewModel.DesignInstance(SubModelClockProgram.DesignModel, SubModelClockProgram.Bindings());
        public object SubModelCounterWithClock => ViewModel.DesignInstance(SubModelCounterWithClockProgram.DesignModel, SubModelCounterWithClockProgram.Bindings());
        public object SubModelOpt => ViewModel.DesignInstance(SubModelOptProgram.DesignModel, SubModelOptProgram.Program);
        public object SubModelOptForm1 => ViewModel.DesignInstance(SubModelOptForm1Program.DesignModel, SubModelOptForm1Program.Bindings());
        public object SubModelOptForm2 => ViewModel.DesignInstance(SubModelOptForm2Program.DesignModel, SubModelOptForm2Program.Bindings());
        public object SubModelSelectedItem => ViewModel.DesignInstance(SubModelSelectedItemProgram.DesignModel, SubModelSelectedItemProgram.Program);
        public object SubModelSeq => ViewModel.DesignInstance(SubModelSeqProgram.DesignModel, SubModelSeqProgram.Program);
        public object UiBoundCmdParam => ViewModel.DesignInstance(UiBoundCmdParamProgram.DesignModel, UiBoundCmdParamProgram.Program);
        public object Validation => ViewModel.DesignInstance(ValidationProgram.DesignModel, ValidationProgram.Program);
        public object FileDialogs => ViewModel.DesignInstance(FileDialogsProgram.DesignModel, FileDialogsProgram.Program);
        public object FileDialogsCmdMsg => ViewModel.DesignInstance(FileDialogsCmdMsgProgram.DesignModel, FileDialogsCmdMsgProgram.Program);
        public object EventBindingsAndBehaviors => ViewModel.DesignInstance(EventBindingsAndBehaviorsProgram.DesignModel, EventBindingsAndBehaviorsProgram.Program);
        public object NewWindow => ViewModel.DesignInstance(NewWindowProgram.DesignModel, NewWindowProgram.Bindings);
        public object NewWindow1 => ViewModel.DesignInstance(NewWindow1Program.DesignModel, NewWindow1Program.Bindings());
        public object NewWindow2 => ViewModel.DesignInstance(NewWindow2Program.DesignModel, NewWindow2Program.Bindings());
    }
}
