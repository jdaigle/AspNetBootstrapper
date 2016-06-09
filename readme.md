This is an example of using a dedicated bootstrapping exe (ApplicationHostBootstrapper)
to run an "ASP.NET Service".

An "ASP.NET Service" a class library which contains initialization code
which often creates and starts a long-lived background worker thread. By
convention, the bootstrapping code implements an interface.

In IIS, we can configure the application preload to execute some common
bootstrapping code which scans for an executes concrete implementations
of this bootstrapping interface.

For local debugging, we have a special ApplicationHostBootstrapper.exe
which creates an AppDomain for the "service", using the Web.config, and
calls the same common bootstrapping code.