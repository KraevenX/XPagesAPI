using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class with common methods
/// </summary>
public static class Common
{

	/// <summary>
    /// Get formatted string from the provided exception 
    /// </summary>
    /// <param name="ex"></param>
    /// <returns></returns>
    [System.Diagnostics.DebuggerStepThrough()]
	public static string GetErrorInfo(Exception ex)
	{
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(ex, true);
		string FileName = "";
		string Method = "";
		string LineNumber = "";
		sb.AppendLine("");
		sb.AppendLine(ex.Message);
		sb.AppendLine("");
		foreach (System.Diagnostics.StackFrame frame in st.GetFrames()) {
			FileName = System.IO.Path.GetFileName(frame.GetFileName());
			Method = frame.GetMethod().ToString();
			LineNumber = frame.GetFileLineNumber().ToString();
			if (FileName != "")
				sb.AppendLine("Filename : " + FileName);
			if (Method != "")
				sb.AppendLine("Method : " + Method);
			if (LineNumber != "")
				sb.AppendLine("Line N° : " + LineNumber);
		}
		return sb.ToString();
	}


    /// <summary>
    /// Get a string from a list of string separated by the provided separator
    /// </summary>
    /// <param name="arList"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    public static string GetListAsString(IList arList, string separator)
	{
		string returnString = "";
		if (arList != null && arList.Count >0) {
			foreach (string str in arList) {
				returnString = returnString + str + separator;
			}
			returnString = returnString.Substring(0, returnString.LastIndexOf(separator));
		}
		return returnString;
	}

    /// <summary>
    /// Application Uncaught Exception Handler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    [System.Diagnostics.DebuggerStepThrough()]
    public static void ExceptionHandler(object sender, System.UnhandledExceptionEventArgs e)
    {
        Exception Excep;
        Excep = (Exception) e.ExceptionObject;
        Connector.ResetReturn();
        Connector.ReturnMessages.Add("Unhandled error occured in application: " + Environment.NewLine + "Application aborted on " + DateTime.Now +Environment.NewLine + GetErrorInfo(Excep));
        Connector.hasError = true;
    
        GC.Collect();
    }
}
