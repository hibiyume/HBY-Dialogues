using Ink.Runtime;

public class InkExternalFunctions
{
    //Usage:
    //   Create function.
    //   Add binding to Bind().
    //   Add unbinding to Unbind();
    
    public void Bind(Story story)
    {
        //story.BindExternalFunction("functionName", (string functionArgs) => FunctionName(functionArgs));
        //story.BindExternalFunction("functionName2", (string functionArgs) => FunctionName2(functionArgs));
    }

    public void Unbind(Story story)
    {
        //story.UnbindExternalFunction("functionName");
        //story.UnbindExternalFunction("functionName2");
    }

    //public void FunctionName(string functionArgs) {  }
    
    
}