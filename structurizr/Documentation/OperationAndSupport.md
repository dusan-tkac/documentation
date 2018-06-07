### Monitoring

#### Errors

Most AMS and Runs Controller components create log entries using [Serilog](https://serilog.net/) and send them to Seq(https://getseq.net/) instance.

Our Seq instance URL is [http://iorper-webd01:5341/#/dash](http://iorper-webd01:5341/#/dash).

The Seq dashboard provides and overview of logged errors and other important events.

<!--![Seq Dashboard](Screenshots/SeqDashboard.png) -->
![Seq Dashboard](https://raw.githubusercontent.com/dusan-tkac/documentation/master/structurizr/Documentation/Screenshots/SeqDashboard.png)

By clicking on a dashboard chart you can navigate to the events screen and see filtered events.

<!--![Seq Events](Screenshots/SeqEvents.png) -->
![Seq Events](https://raw.githubusercontent.com/dusan-tkac/documentation/master/structurizr/Documentation/Screenshots/SeqEvents.png)

Errors monitoring should be done daily.

Seq also stores other event types, not just errors. These logged events can be invaluable when investigating some issues.

#### Runs Controller status

Another daily task is to check availability of Runs Controller clients.

Runs Controller Portal Nodes screens lists active, inactive and decommissioned nodes (clients).
If client becomes inactive it usually means there are technical issues on the machine that need to fixed.

Number of threads displayed on the dial in the top-right corner is another useful indicator.

This screen also displays distributor status.

<!--![Runs Controller Nodes](Screenshots/RunsControllerNodes.png) -->
![Runs Controller Nodes](https://raw.githubusercontent.com/dusan-tkac/documentation/master/structurizr/Documentation/Screenshots/RunsControllerNodes.png)

Runs controller distributor and clients still use event log for most of their logging.
In some cases it may help to have a look at logged events when investigating Runs Controller issues.

#### Hangfire Queue

Hangfire also requires regular monitor.

Occasionally, most often in Test environment, job gets stuck and blocks processing queue.
The stuck job needs to be deleted; sometimes, if the queue has grown too long; the enqueued jobs need to be deleted too.

The Hangfire Windows Service may need a restart.

Recently deleted jobs can then be re-queued.

<!--![Hangfire - deleted jobs](Screenshots/HangfireDeletedJobs.png) -->
![Hangfire - deleted jobs](https://raw.githubusercontent.com/dusan-tkac/documentation/master/structurizr/Documentation/Screenshots/HangfireDeletedJobs.png)


#### Database sizes

All database sizes and their growth should be regularly checked to avoid running out of space.