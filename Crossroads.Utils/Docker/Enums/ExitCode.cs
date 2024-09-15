using System.ComponentModel;

namespace Crossroads.Utils.Docker.Enums;

public enum ExitCode
{
    [Description("Container was automatically stopped")]
    Exit = 0,
    
    [Description("Container was stopped due to application error or incorrect reference in the image specification")]
    ApplicationError = 1,
    
    [Description("The docker run command did not execute successfully")]
    FailedToRun = 125,
    
    [Description("A command specified in the image specification could not be invoked")]
    CommandInvokeError = 126,
    
    [Description("File or directory specified in the image specification was not found")]
    FileOrDirNotFound = 127,
    
    [Description("Exit was triggered with an invalid exit code")]
    InvalidArgOnExit = 128,
    
    [Description("The container aborted itself using the abort() function")]
    Sigabrt = 134,
    
    [Description("Container was immediately terminated by the operating system via SIGKILL signal")]
    Sigkill = 137,
    
    [Description("Container attempted to access memory that was not assigned to it and was terminated")]
    Sigsegv = 139,
    
    [Description("Container received warning that it was about to be terminated, then terminated")]
    Sigterm = 143,
    
    [Description("Container exited, returning an exit code outside the acceptable range, meaning the cause of the error is not known")]
    Unknown = 255
}