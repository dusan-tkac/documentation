### Hangfire service and AMS Backend Development

Hangfire service (AMS service) and AMS are kept within single Visual Studio Solution. They use same services and repositories.

AMS still uses LightSwitch which is not being actively developed anymore. Do not create anymore functionality dependent on LightSwitch.

#### Data Access

Database structure is defined, maintained and deployed using Database project in Visual Studio.

We use Entity Framework 6 Code First to access the database from non-LightSwitch parts of AMS.

We manage Entity Framework DBContext instances using an approach (and library) created by Mehdi El Gueddari: [Managing DbContext the right way with Entity Framework 6: an in-depth guide](http://mehdi.me/ambient-dbcontext-in-ef6/)

We only use stored procedures where it is absolutely necessary.

We prefer not to use lazy loading of referenced entities.

Some collection virtual properties were intentionally removed to motivate developers to find better (faster) way to load data in bulk.

#### Dependency injection

We use [Ninject](http://www.ninject.org/).

There is a separate definition of available services for AMS API and Hangfire Service.

#### Code organisation

Contracts (data contracts, models and interfaces) are kept in a separate project.

Logic should be implemented in services, data access (including cache management) should be in repositories.

Controllers should be lightweight.

We do not do REST but there is no harm in getting close to it where practical.

#### Coding style

Comment where necessary.

Use self-explanatory names for classes, methods, properties and variables - we prefer longer names that make sense.

Log errors and important events using [Serilog](https://serilog.net/).

### Runs Controller development

Runs Controller has quite large code base considering it's functionality

Distributor implementation is very complex with multiple layers of services and rather large amount of unused code.

Despite this complexity it works quite well.

Thread with caution when attempting to make changes.

It is likely to be replaced soon so no radical changes are necessary or desirable anymore.