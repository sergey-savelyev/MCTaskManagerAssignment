## IN PROGRESS

**I'm currently migrating to a clean architecture and optional DynamoDB storage implementation.**

Stay tuned!

## CHANGELOG

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

To run the application on your machine, you'll need Docker installed. Clone the repository, navigate to it's root directory, and run the command `docker compose up --build`. The deployment will take some time—just enough to make yourself a cup of coffee!

After the deployment is complete and all three containers are running, go to `localhost:3939` in your browser. This will take you to the main screen of the application.

## Using the Application

- On the main page, there are 2 tabs: "Tasks" and "Logs."
  ![Main screen](docs/screenshot-1.png?raw=true "Main screen")
- By default, the "Tasks" tab is selected, and it should be empty for you if you run the app for the fist time. To create your first task, you'll need to click on the corresponding button in the top left corner.
- You can edit an already created task. Click "assign?" if you want to assign the root task for the task you're editing. Keep in mind that the opened search window looks for both the summary and the description of the task.
  ![Task details screen](docs/screenshot-2.png?raw=true "Task details screen")
- The application will not allow you to create a loop in the chain by assigning one of the sub-tasks as a parent to it's root task. You can navigate through the task chain by clicking on the root tasks in the upper part of the modal window or on the sub-tasks in the lower part.
- Please note that the list displays **ONLY top-level** tasks that have **no root tasks**. You can sort the list by clicking on the column headers.

## Technical Details

### Challenges I encountered:

1. **Choosing a database:**
   The data structure for such a task is quite obvious and comes to mind from the very first minutes. We need a collection of entities organized as a singly linked list. Each task should store a reference to its parent task (if any). Parent tasks, however, know nothing about their subtasks.
   Initially, Amazon DynamoDB seemed like a good choice for this, and I even started implementing it on this database. But then, I came across a moment.

2. **Sorting:**
   There's a sorting by task fields requirement in the assignment specifications. In general, there are 2 approaches. The bad one: We could load all tasks from db and sort them in the memory, providing the result. Extremely unefficient and not scalable. Clearly, data should be sorted on the database side. And here is where Amazon DynamoDB starts to pose difficulties. Initially designed for fast and specific read-write operations, this database doesn't work very well with aggregation and sorting. To ensure sorting by all other fields, I had to create the maximum allowable number of local indexes or several global ones. For such an application and functionality, these maneuvers seemed like overkill. It was decided to return to tried-and-true technologies: time-tested MySQL. Let it, on average, work a bit slower and bear the costs in the form of scoped services instead of familiar singletons; for this task, it seems to be the most suitable option.

3. **Fighting task looping:**
   Initially, it was clear that making task a parent and a child for the same task was a bad idea. When binding tasks between each other, what we needed was to gracefully "untangle" the chain when creating a new link and ensure that the potential parent is not already our subtask. Moreover, this needed to be done without resorting to multiple requests through recursion. You never know how many gazillions of tasks could end up in one chain; users can be quite inventive! I solved the problem by moving the recursive traversal through the chain to the database side, using Recursive Common Table Expressions (CTE). Fast and efficient!

4. **Indexing:**
   To facilitate convenient task search, I applied full-text indexes to the "summary" and "description" fields.

5. **Frontend:**
   It's always a challenge for a backend developer!

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
