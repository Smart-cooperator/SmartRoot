using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Utilities;

namespace wfca
{

    public sealed class CommandActivity : CodeActivity
    {
        // Define an activity input argument of type string
        public InArgument<string> WorkDirectory { get; set; }

        public InArgument<string> Cmd { get; set; }

        public OutArgument<int> ExitCode { get; set; }

        public OutArgument<string> StandOutput { get; set; }

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument
            //string text = context.GetValue(this.Text);
            string workDirectory = context.GetValue(WorkDirectory);
            string cmd = context.GetValue(Cmd);

            Command.Run(workDirectory, cmd, out int exitCode, out string standOutput, out string errorOutput);

            context.SetValue(ExitCode, exitCode);
            context.SetValue(StandOutput, standOutput);
        }
    }
}
