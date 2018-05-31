### AMS Functional Overview

The main function of AMS is to manage and allow for editing of model inputs and to allow user to create (P2C) submissions.

Values, that are used as model inputs, are organized into a structure called **state**.

States are used as input states for cases. **Case** (model case) binds together an input state and model version. Users create submissions for cases - they specify model run configuration (submission parameters) and number of seeds (packages) in the submission.

Cases are further grouped into **scenarios** and scenarios are grouped into **projects**.

To summarize this hierarchy:

* projects have many scenarios
* scenarios have many cases
* case has one input state
* case can have many output states (however, this feature in not being used)
* case has one model version
* case has one or more model submissions (usually one)
* submission has many seeds (packages)

> Output states in AMS were supposed to hold "important" model outputs and AMS was supposed to provide basic comparison of input state and output state(s).
> This functionality was never implemented in AMS.

#### Management and editing of model inputs

##### Project hierarchy browsing and editing

AMS provides browse and edit screens for all entities of the hierarchy as listed above.

<!-- ![Browse States](Screenshots/BrowseStates.png) -->
![Browse States](https://raw.githubusercontent.com/dusan-tkac/documentation/master/structurizr/Documentation/Screenshots/BrowseStates.png)
<!-- ![Add State](Screenshots/AddState.png) -->
![Add State](https://raw.githubusercontent.com/dusan-tkac/documentation/master/structurizr/Documentation/Screenshots/AddState.png)

Users can create instances of all these entities "manually" using edit screens or they can upload their definitions using Excel spreadsheet upload.
States can be also upload from Excel spreadsheet.
Excel spreadsheet upload also allows user to create model submissions.

##### Bulk upload for scenarios, cases, submissions and states

<!-- ![Upload States, Scenarios, Cases and Submissions](Screenshots/Upload.png) -->
![Upload States, Scenarios, Cases and Submissions](https://raw.githubusercontent.com/dusan-tkac/documentation/master/structurizr/Documentation/Screenshots/Upload.png)
<!-- ![Upload Template](Screenshots/UploadTemplate.png) -->
![Upload Template](https://raw.githubusercontent.com/dusan-tkac/documentation/master/structurizr/Documentation/Screenshots/UploadTemplate.png)

##### Change sets

AMS supports a special kind of state - **change set**
Change sets are not used as model inputs. Instead they can be "applied" to another states changing (replacing or removing) their content.
<!-- ![Apply Change Set](Screenshots/ApplyChangeSet.png) -->
![Apply Change Set](https://raw.githubusercontent.com/dusan-tkac/documentation/master/structurizr/Documentation/Screenshots/ApplyChangeSet.png)

Change sets creation and "application" can be also done via Excel spreadsheet upload.

##### State summary

State values can be edited directly in AMS.

The "State Summary" screen is a general-purpose editing tool for editing of "atomic" values.

<!-- ![State Summary](Screenshots/StateSummary.png) -->
![State Summary](https://raw.githubusercontent.com/dusan-tkac/documentation/master/structurizr/Documentation/Screenshots/StateSummary.png)

##### Case setup tool

Another tools that was supposed to work across all value structures is Case Setup Tool. The main idea behind this tool is to a way to generate new states from a selected base state where only selected values will be modified in a specified way in the new states.
This, however, currently does not work for nested values (see object type system bellow) and this tool is being replaced with a new version.

<!-- ![Case Setup Tool](Screenshots/CaseSetupTool.png) -->
![Case Setup Tool](https://raw.githubusercontent.com/dusan-tkac/documentation/master/structurizr/Documentation/Screenshots/CaseSetupTool.png)

##### Specialized tools for state editing

AMS also contains a collection of specialized tools that are used for editing of standardized data structures frequently occurring in states.

These tools are:

* Stockyard Layout Editor - as the name says, this is used for editing of stockyard definitions
* Outage Plans Editor - editing of outage plans and maintenance plans
* Marine Paths Editor
* Mine Port Sequence Editor
* Marine Paths Editor

<!-- ![Stockyard Layout Editor](Screenshots/StockyardLayoutEditor.png) -->
![Stockyard Layout Editor](https://raw.githubusercontent.com/dusan-tkac/documentation/master/structurizr/Documentation/Screenshots/StockyardLayoutEditor.png)

All these specialized tools rely on parts of state being organized in a specific way - the values must be organized using specific object types and properties (see object type system description later in this document).

##### Case Matrix

Case Matrix is a tool that allows user to easily create new cases, set their input states and execute them (create submissions and push them to Runs Controller) in bulk.

<!--![Case Matrix](Screenshots/CaseMatrix.png) -->
![Case Matrix](https://raw.githubusercontent.com/dusan-tkac/documentation/master/structurizr/Documentation/Screenshots/CaseMatrix.png)

##### Asset Maps

Asset Maps allow user to easily see most important values in their state. It can not only display atomic values but also calculated values.

It displays state objects and their values (including calculated values) in layout defined by user.

Asset maps are fully configurable by the user.

<!--![Asset Maps](Screenshots/AssetMap.png) -->
![Asset Maps](https://raw.githubusercontent.com/dusan-tkac/documentation/master/structurizr/Documentation/Screenshots/AssetMap.png)

##### State Validation

State Validation displays same value as Asset Maps - it uses configured asset maps to retrieve the list of values as well.
The values are displayed in the table and it also allows to display values from multiple state side-by-side allowing for their comparison.

Users can also record their comments for each value.

<!--![State Validation - selection of maps and states](Screenshots/StateValidationSelection.png) -->
![State Validation - selection of maps and states](https://raw.githubusercontent.com/dusan-tkac/documentation/master/structurizr/Documentation/Screenshots/StateValidationSelection.png)

<!--![State Validation - comparing values](Screenshots/StateValidationResults.png) -->
![State Validation - comparing values](https://raw.githubusercontent.com/dusan-tkac/documentation/master/structurizr/Documentation/Screenshots/StateValidationResults.png)

##### State Comparison

State comparison displays two selected states side-by-side in the tree view format (same as state summary) and highlights differences.

User can also export the difference between states as a change set.

<!--![State Comparison](Screenshots/StateComparison.png) -->
![State Comparison](https://raw.githubusercontent.com/dusan-tkac/documentation/master/structurizr/Documentation/Screenshots/StateComparison.png)

##### Actual Data Import

This screen allows for import of actual data into AMS in the form of change sets.

Actual data are coming to AMS as results from configured stored procedures.

Users can define new procedures to be used too.

The import itself is executed as a background job by Hangfire.

<!--![Import actual data as a change set](Screenshots/ImportActualDataToChangeset.png) -->
![Import actual data as a change set](https://raw.githubusercontent.com/dusan-tkac/documentation/master/structurizr/Documentation/Screenshots/ImportActualDataToChangeset.png)

#### Object type system

Values in AMS are organized within states into hierarchies.
Structure (schema) of the hierarchy is defined using object type system.

##### Introduction to AMS object type system

The AMS object type system design was inspired by object-oriented design.

Elements of the object type system are:

* object types
* properties
* property options (if property represents an enumeration of possible values)
* object type properties
* data types
* units of measures

Each value in AMS has to "belong" to an object.
Objects are "instances" of object types (e.g. "Car Dumper 1" is an instance of the "Car Dumper" object type).
Each object type has a list of object type properties (e.g object type "Car Dumper" has object type properties "Car Dumper Net Rate", "Car Dumper Throughput" etc.).
Each object type property references a property. Property represent a well-known term that can be used for multiple object types (e.g. object type property "Car Dumper Net Rate" references property "Net Rate". Property "Net Rate" is also used for other types of equipment.)

Each value then references an object and an object type property.
> In fact there is a separate entity that represents this relationship called object property. 
> This entity is technically not needed anymore but remains in AMS for historical reasons.

Object types in AMS represent various equipment types, nested data structures (e.g. distributions), model configuration that needs to be a part of an input state etc.

There are two categories of object types in AMS:

* "Not-owned"
* "Owned"

Object type is **not-owned** if its instances (objects of this type) need to have an identity (e.g. car dumpers need to have an identity - unique identifier - so that they can be tell apart). These object with identity (not-owned objects) are sometimes called top-level objects because in state summary they are at the top of the hierarchy.

Object type is **owned** if its instances don't have to (or can't) have an identity (e.g. distribution objects don't have to have an identity, because distribution objects are always used as a values of some object type property.) Owned objects are always used as values of some object type property. It can be a property of not-owned or owned object, but there always is a "parent" for such object. These objects are "nested" in the hierarchy.

Another feature of AMS object type system is a support of inheritance of object types.
An object type can be used as a parent for another object type(s). This derived type then "inherits" object type properties of its parent.

AMS also supports arrays. All arrays in AMS are arrays of objects. These object can be not-owned (then it's really an array of object references) or owned (and then owned objects are really nested in the array with all their values). Arrays are either defined as homogeneous (all elements are of the same object type (or have same parent object type)) or heterogeneous (array elements can be of various types).
Array elements have 1-based indices.

##### Maintenance of object type system

AMS users have an option to create new or change existing

* object types
* object type properties
* properties
* property options
* units of measure

> object properties are created automatically
> 
> data types are fixed and cannot be modified

There are standard browse and edit screens for all entities listed above.

There is also a special Object Type Viewer/Editor available in AMS.

<!-- ![Object Type Viewer/Editor](Screenshots/ObjectTypeViewer.png) -->
![Object Type Viewer/Editor](https://raw.githubusercontent.com/dusan-tkac/documentation/master/structurizr/Documentation/Screenshots/ObjectTypeViewer.png)

#### Knowledge Items

Current version AMS was built with traceability being one of the most important drivers.
The original requirement was for each "piece of data" entered into AMS to have a attached proof of source - knowledge item.

AMS therefore contains comprehensive set of functionality for defining and using knowledge items for nearly all data entities.

Knowledge items have

* type (e.g Definition, Calculation)
* category (e.g. IPS, DPS, Acquired)
* data source type (e.g. Document, 1SAP, MQ2)
* calculation tool/method (e.g. Excel Template, Spotfire, Matlab Script)

There is also a configuration matrix defining which of what these attributes of a knowledge item can be so the knowledge item can be attached to a particular data entity.

The rules on knowledge items usage have since been relaxed, but knowledge item must still be referenced when importing state data to AMS.

<!--![Knowledge Item](Screenshots/KnowledgeItem.png) -->
![Knowledge Item](https://raw.githubusercontent.com/dusan-tkac/documentation/master/structurizr/Documentation/Screenshots/KnowledgeItem.png)

#### Control Framework Configuration

Control Framework is the original solution for background processing and recurrent jobs.
Its functionality is implemented as a set of SSIS packages that are executed by SQL Server jobs with supporting collection of stored procedures and configuration tables.

AMS provide user interface for Control Framework configuration.

Some of the original background and recurrent jobs were moved from Control Framework to Hangfire.
The remaining jobs mostly load data to Analytical Data Storage - either from external systems or from Models Outputs Storage.

[ ] TODO Create and reference Control Framework documentation

#### Model Versions

AMS data structures in theory support multiple model types, models and model platforms.

In practice, the model that is being used is P2C (Pit to Customer).

Versions of this model can be created in AMS "manually" using Model Version Edit screen, but they are typically imported to AMS by model developers directly from their development environment.

AMS does not actually store model executable files or other artifacts. It only stores model version "meta-data" such as a list of parent model versions and list of model version parameters.

The list of parameters is used to dynamically generate user interface for submission creation or to validate submission parameters if submissions are imported from Excel spreadsheet.

<!--![Model Version Details](Screenshots/ModelVersionDetails.png) -->
![Model Version Details](https://raw.githubusercontent.com/dusan-tkac/documentation/master/structurizr/Documentation/Screenshots/ModelVersionDetails.png)

<!--![Dynamically generated parameters based on Model Version definition](Screenshots/ModelVersionParameters.png) -->
![Dynamically generated parameters based on Model Version definition](https://raw.githubusercontent.com/dusan-tkac/documentation/master/structurizr/Documentation/Screenshots/ModelVersionParameters.png)

#### Submission Progress

Submission Progress screen allows user to search through existing submissions and then check their status as they progress through the processing stages.

<!--![Submission Progress - Search](Screenshots/SubmissionProgressSearch.png) -->
![Submission Progress - Search](https://raw.githubusercontent.com/dusan-tkac/documentation/master/structurizr/Documentation/Screenshots/SubmissionProgressSearch.png)

Details of the submission progress show a list of submission packages with links to package details in Runs Controller Portal.

It also shows which package outputs have been registered; if post-processing package has been created and what is it's status.

It also shows which pot-processed outputs where loaded to database (or Model Outputs Storage).

<!--![Submission Progress - Search](Screenshots/SubmissionProgressDetails.png) -->
![Submission Progress - Search](https://raw.githubusercontent.com/dusan-tkac/documentation/master/structurizr/Documentation/Screenshots/SubmissionProgressDetails.png)

#### Type System Versions

Type System Versions and modules are an attempt to resolve a problem of AMS inputs states being too "big" - typical state contains tens of thousands of individual values.

Although state have really complex structure, for a particular project only certain groups of values are changed between scenarios and cases.

Also between projects, there are often differences limited to certain groups of values.

Type System Version and its modules is a way to identify and specify these groups of values are often imported together as change sets and that are often copied from one state to another.

*Module* defines such group of values by listing object type properties.

Type system version is then a collection of these modules.

By having an option to create multiple type system versions we support changes in the Object Type System required by new model versions (when new object type properties are created or existing object type properties are made inactive) and changing definitions of modules.

Defined type system version modules can be used to filter displayed parts of a state in state summary and state comparison.
They can be also used to simplify merging of two states.

[] TODO explain why objects and model version can be linked to type system version

#### Merging States using Modules

#### Other Reference Data

* Customers
* Project Types

#### Navigation

* Dashboard
  * overview of queued, running and recently finished submissions with links to Runs Controller web site for current user and all users
  * navigation to main browse screens (projects, scenarios, cases and states)
  * navigation to bulk import screens (states and submissions)
  * navigation to user's favorite entities
* Browse screens for main entities (Projects, Scenarios, Cases, States)
* State validation

#### Hangfire (AMS Service) Functional Overview

### Runs Controller Functional Overview

This is functional overview