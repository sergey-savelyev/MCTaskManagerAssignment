## IN PROGRESS

**Curently working on reimplementation of frontend using Svelte**

Stay tuned!

## CHANGELOG

#### 03/10/2023 Update:
- Full DynamoDB support implemented. Required some optimizations though

#### 02/10/2023 Update:
- Significant architecture improvements according to the covenants of Clean Architecture

#### 01/10/2023 Update:
- Namespaces improvements;
- Task actions logging logic refactoring: redundand services removed;
- Minor general refactoring and improvements;

#### 30/09/2023 Update:
- Upsert task split to two methods: Update task and Create task;
- RootId of the task can no longer be changed via Update method. Use Change Task Root method instead;
- Redundant repository classes and interfaces were removed;
- Optimized utilization of DbContext class;
- Minor general refactoring and improvements.


# Welcome aboard!

If you're reading this, you've probably come to check out my test assignment. Since this repository is public, I won't mention specific names and titles. So, welcome aboard!

## The Task

The task was to create a task manager, in other words, a to-do list. The user should be able to create, edit, and delete tasks. The key feature, however, is that tasks can be infinitely nested within each other. Additionally, if desired, logging of all actions could be implemented.

## Running the Application

To run the application on your machine, you'll need Docker installed. First, clone the repository, navigate to it's root directory. 

The app supports both build with MySQL and build with DynamoDB.

### Running MySQL build

`docker compose up --build`

### Running DynamoDB build

`docker compose -f docker-compose.dynamodb.yml up --build`

### Make yourself a cup of coffee or tea! You're awesome!

After the deployment is complete and all three containers are running, go to `localhost:3939` in your browser. This will take you to the main screen of the application.

# Why and how

## DynamoDB and MySQL
In the assignment requirements there was one about the database. I could implement the storage using DynamoDB and MySQL. Since I don't have much experience with Dynamo DB, I went with MySQL. But I couldn't stop just like that. I took a course at Pluralsight, learned a bit about DynamoDB and decided to implement it as well. Sure thing it's not the best implementation and some things could be optimized much better (like schema and usage of batch requests). Maybe I'll improve it soon, maybe I'll leave it for the future :) 

## Transactions
I brought this topic with quite untypical statement. Actually, what I want to explain is why this app **DOESN'T** need transactions.

Well, the answer is pretty much simple: it would be overkill for the sake of nothing. In a perfect world, this app would be used by many users, each of which has their own "notebook" with tasks. So, consistency remains in a domain of one unique user unless we decide to share tasks. But this functionality is out of this assignment.

# Technical explanation

The app split for 4 layers: Core (a.k.a. Domain), Application, Infrastructure and API (a.k.a. Presentation). There's also a client written with JS.

## Core (a.k.a. Domain)

This layer contains the essentials of the app: basic data structures and interfaces. It also has no dependencies for sure.

## Application

This layer depends on Core and contains the essential bussiness logic, DTOs that Presentation could use and infrastructure interfaces.

## Infrastructure

This layer depends on Application and contains implementation of DBs. There are two projects: for MySQL and DynamoDB.

## API (a.k.a. Presentation)

This layer depends on Infrastrucure and Application and implements the API to manipulate the logic.

## Client (additional stuff)

This one is horrible and has never been refactord properly. I don't really care about it, but if you have a chance, it's better not to read the code of it.

## Using the Application

- On the main page, there are 2 tabs: "Tasks" and "Logs."
  ![Main screen](docs/screenshot-1.png?raw=true "Main screen")
- By default, the "Tasks" tab is selected, and it should be empty for you if you run the app for the fist time. To create your first task, you'll need to click on the corresponding button in the top left corner.
- You can edit an already created task. Click "assign?" if you want to assign the root task for the task you're editing. Keep in mind that the opened search window looks for both the summary and the description of the task.
  ![Task details screen](docs/screenshot-2.png?raw=true "Task details screen")
- The application will not allow you to create a loop in the chain by assigning one of the sub-tasks as a parent to it's root task. You can navigate through the task chain by clicking on the root tasks in the upper part of the modal window or on the sub-tasks in the lower part.
- Please note that the list displays **ONLY top-level** tasks that have **no root tasks**. You can sort the list by clicking on the column headers.

### API Overview

The Task API allows users to manage tasks and perform various operations related to tasks.

### Task API

#### Get Task by ID

- **URL**: `/api/tasks/{taskId}`
- **Method**: GET
- **Parameters**:
  - `{taskId}`: Task ID to retrieve.
- **Response**:
  - 200 OK: Returns the task with the specified ID.
  - 404 Not Found: If the task with the given ID is not found.

#### Search Tasks

- **URL**: `/api/tasks/search/{phrase}`
- **Method**: GET
- **Parameters**:
  - `{phrase}`: Phrase to search for in tasks.
  - `take`: Number of items to retrieve (default: 20).
  - `skip`: Number of items to skip (default: 0).
- **Response**:
  - 200 OK: Returns a list of tasks matching the search phrase.

#### Get Root Task Batch

- **URL**: `/api/tasks`
- **Method**: GET
- **Parameters**:
  - `take`: Number of items to retrieve (default: 20).
  - `skip`: Number of items to skip (default: 0).
  - `sortBy`: Field to sort by (default: "CreateDate").
  - `descendingSort`: Sort in descending order (default: false).
- **Response**:
  - 200 OK: Returns a list of root tasks.

#### Create Task

- **URL**: `/api/tasks`
- **Method**: POST
- **Parameters**:
  - `taskData` (Request Body): Task data to create a task.
    - `Summary` (string, required): Summary of the task.
    - `Priority` (enum, required): Task priority. Possible values: "High", "Medium", "Low".
    - `Status` (enum, required): Task status. Possible values: "NotStarted", "InProgress", "Completed".
    - `Description` (string, optional): Description of the task.
    - `DueDate` (string, required): Due date of the task.
- **Response**:
  - 200 OK: Returns the ID of the created task.

#### Update Task

- **URL**: `/api/tasks/{taskId}`
- **Method**: POST
- **Parameters**:
  - `{taskId}`: Task ID to update.
  - `taskData` (Request Body): Task data to create or update a task.
    - `Summary` (string, required): Summary of the task.
    - `Priority` (enum, required): Task priority. Possible values: "High", "Medium", "Low".
    - `Status` (enum, required): Task status. Possible values: "NotStarted", "InProgress", "Completed".
    - `Description` (string, optional): Description of the task.
    - `DueDate` (string, required): Due date of the task.
- **Response**:
  - 200 OK: The task successfully updated.

#### Change Task Root

- **URL**: `/api/tasks/{taskId}/root`
- **Method**: PATCH
- **Parameters**:
  - `{taskId}`: Task ID to update.
  - `newRoot`: New root data for the task.
- **Response**:
  - 204 No Content: Successful update of the task's root.
  - 400 Bad Request: If the root update is invalid.

#### Delete Task

- **URL**: `/api/tasks/{taskId}`
- **Method**: DELETE
- **Parameters**:
  - `{taskId}`: Task ID to delete.
- **Response**:
  - 204 No Content: Successful deletion of the task.
  - 404 Not Found: If the task with the given ID is not found.

### Task Logs API

#### Get Task Logs by Task ID

- **URL**: `/api/tasks/{taskId}/logs`
- **Method**: GET
- **Parameters**:
  - `{taskId}`: Task ID to retrieve logs for.
  - `skip`: Number of items to skip (default: 0).
  - `take`: Number of items to retrieve (default: 20).
  - `orderby`: Field to order by (default: "TimestampMsec").
  - `descending`: Sort in descending order (default: true).
- **Response**:
  - 200 OK: Returns task action logs for the specified task.

#### Get All Logs

- **URL**: `/api/tasks/logs`
- **Method**: GET
- **Parameters**:
  - `skip`: Number of items to skip (default: 0).
  - `take`: Number of items to retrieve (default: 20).
  - `orderby`: Field to order by (default: "TimestampMsec").
  - `descending`: Sort in descending order (default: true).
- **Response**:
  - 200 OK: Returns task action logs.

## This is it

Feel free to ask any questions you have! And if you want to challenge me in any way, by adding or improving some functionality, let me know!

Thank you so much for your time!

Sincerely, Sergei
