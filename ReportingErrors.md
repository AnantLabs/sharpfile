# Introduction #
I always welcome issues/bugs that are filed through Google Code's interface. Users can also use the Google Group set up for SharpFile. I usually try to respond within a couple of days of any issue.

# Attaching log file #
If the bug can be replicated, it might be beneficial to attach a debug log file with the bug report.

In the settings.config, change the loglevel to "Verbose" and then attach the log.txt file to your bug to give me more details. Normally, the settings file would have this:
```
<Logger>
 <File>log.txt</File>
 <LogLevel>ErrorsOnly</LogLevel>
</Logger>
```

Change it to:
```
<Logger>
 <File>log.txt</File>
 <LogLevel>Verbose</LogLevel>
</Logger>
```

Then, run the program and generate the error. Attach the log.txt file to your bug and it will be of great help in tracking down the issue. Thanks!