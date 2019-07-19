/**
*    Copyright(C) G1ANT Ltd, All rights reserved
*    Solution G1ANT.Addon, Project G1ANT.Addon.IExplorer
*    www.g1ant.com
*
*    Licensed under the G1ANT license.
*    See License.txt file in the project root for full license information.
*
*/
using G1ANT.Language;
using System;

namespace G1ANT.Addon.IExplorer
{
    [Command(Name = "ie.runscript", Tooltip = "This command executes script on the currently attached Internet Explorer instance")]
    public class IERunScriptCommand : Command
    {
        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "Script to be evaluated in a browser")]
            public TextStructure Script { get; set; }

            [Argument(Tooltip = "Name of a variable where the script result will be stored")]
            public VariableStructure Result { get; set; } = new VariableStructure("result");

            [Argument(Tooltip = "If set to `true`, the script will continue without waiting for a webpage to respond to a runscript")]
            public BooleanStructure NoWait { get; set; } = new BooleanStructure(false);
        }
        public IERunScriptCommand(AbstractScripter scripter) : base(scripter)
        {
        }
        public void Execute(Arguments arguments)
        {
            try
            {
                IEWrapper ie = IEManager.CurrentIE;
                if(arguments.NoWait.Value)
                {
                    arguments.Script.Value = $"setTimeout(function() {{ {arguments.Script.Value} }}, 0) ";
                }
                string result = ie.InsertJavaScriptAndTakeResult(arguments.Script.Value);
                Scripter.Variables.SetVariableValue(arguments.Result.Value, new TextStructure(result));
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Problem occured while trying to run javascript code. Message: {ex.Message}", ex);
            }
        }
    }
}
