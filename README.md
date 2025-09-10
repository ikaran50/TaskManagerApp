**Project Description**: This is a TaskManager application which is built using .Net core in the server side using SQLLITE+ EFCORE DB. The UI uses react.js with typescript. 
**Setup Instructions**: 
- Make sure you have these preRequesites in order to run the application locally.
- **.NET 8 SDK, Node.js 20**+
  1. Git clone the taskManagerApp repo from github into your local -> git clone git@github.com:ikaran50/TaskManagerApp.git
  2. **Backend Setup:**
     - Open a terminal and go to the location where the backend project is cloned. It should be called TaskManagement.Server
     - Run the following commands
       1. dotnet restore
       2. dotnet tool install --global dotnet-ef
       3. dotnet ef database update (This will create SQLLite DB file in project root)
       4. dotnet run (This will start the server) at http://localhost:5000/api
 3. **FrontEnd Setup:**
    - Open another terminal window and go to the location where the front end project is cloned. It should be in the same folder and called taskmanagement.client
    - Run the following commands
      1. npm install -> You might get an error with fluentui/react-components dependency so in this case install this  seperately by running (npm i @fluentui/react-components)
      2. npm run dev
      3. The app will start at http://localhost:5173
  
**USAGE AND FEATURES**
   1. Once you go to http://localhost:5173 you will see the main page with a taskForm from where you can add Tasks. You need to enter the Title, Description is optional and pick
      the due date from the calendar. Once you click the "Add Task" button you will see the task added below. If you select a due date prior to the current Date it will not let you
      add the task.
   2. You can search using title and description, Filter by All or Open Tasks and also sort by DueDate.
   3. Initially the tasks will be open and you can mark it as complete by clicking the Mark Complete Button. Once the task is marked Complete you will see those tasks in the
      "Completed Tasks" tab. You can also reOpen those tasks because as soon as you mark it complete, it will show the Mark Open option on the tasks. You can open those tasks again
      which will remove the tasks from the Completed Tasks page.
   4. You can also delete tasks and assign those tasks to a active User. To assign it to a active User you first need to create a new user from the Users tab at the Top of the page.
   5. Once you create a new User, click on the "Assign User" button and it will open a dropdown on the right with  only the Active users in the app. Once you select the User
      you will be able to see the userName in the Assignee section on the task.
   6. You can also unAssign the user by clicking unAssign from the same dropDown menu.
   7. Only Active users can be assigned to a task. You can mark a user as inActive from the users page.

**DESIGN SPECS**
   **BACKEND**
   1. The backend has been built using .NET Core and SQLLITE. The DB consists of 2 tables. The tasks table which contains all the tasks info. There is the users table which
      contains all the userInfo. The userID is a nullable foreign Key in the tasks table. This has been done to link the task with the user because each task can be assigned to a
      active User. The reason for being nullable is that initially when the task is added, it is not assigned to a User. So in this case it needs to hold a null value in the DB
      table initally. Also the tasks can be unAssigned which means that the task should have the option to have a user and also not have a user so this design makes sense.
   2. The API design uses REST API which has 5 endpoints each meant for a certain usage in the app. The UI makes a request to these endpoints. 
       1. Get All Tasks: **GET: api/tasks?query=&status=all|open|done&sort=due|created**. When fetching all the tasks it can query using get Request. A sample request would be
          **api/tasks?status=allsort=created**
       2. GetTask by ID: **GET: api/tasks/5** Using 5 as an ID example: you can make this get  request to get task with ID 5.
       3.  **POST: api/tasks**  This is a POST request to create a new task. ID is assigned after task creation
       4.   **PUT: api/tasks/4** This is a put request to update an existing task with an active user. This would assign the task to a user and also unAssign
       5.  **PATCH: api/tasks/5/toggle** This is a patch request to mark the Task complete and reOpen. This is basically a toggle feature
       6.  **DELETE: api/tasks/5** This is a delete request to delete and existing task
        
  3. **Caching**: The server uses caching to cache all the tasks. This is useful for scalability because when the app grows big and there are a lot of tasks, caching would reduce
     the amount of DB calls to improve performance. Essentially, we dont want to make a DB query each time we are fetching the tasks. Caching speeds up the response time.

     **Cache Key** : Caching is a complex design because it involves selecting the right cacheKey. In this case, I am using 2 cacheKeys.
    **tasks_{query}_{status}_{sort} and tasks_{status}_{sort}**. The cacheKey is meant to be unique for each status, sort and query value because otherwise querying or selecting
     the status or sort would return stale data because they would all have the same cacheKey. Also in the case when there is no search query, the cacheKey is kept for 30 seconds.
     Otherwise it is kept for 5 seconds. This is because the cacheKey is unKnown when there is a search because the text that the user enters in unKnown. So its better to inValidate
     the cache immediately to avoid returning stale Data. When there is no query search, the cache is invalidated in 30 seconds.
     
     **Cache Invalidation**: Whenever there is a **POST/PATCH/PUT/DELETE** request being made the cache is invalidated. This is important because if a new task is added or existing task is updated
     the DB changes so the cache needs to be cleared so the GET ALL Tasks request can request the DB for the new data and not use the cache. Not invalidating
     the cache would return stale data.

     
   **FRONTEND**
   1. The frontend comprises of 5 components which render. TaskForm, TaskList, UserListDropDown, UsersForm, UsersList
   2. TaskForm component is responsible for rendering the form to create the new task. TaskList is displaying a list of tasks and also the completed tasks page.
   3. UserListDropDown renders the dropdown to assign the task to an Active User
   4. UsersForm is meant to add the new user and UsersList displays those users.
   5. Another component App.tsx is the main component which renders the 5 subComponents based on the use case.

   **FUTURE IMPROVEMENTS**
   1. The app currently has  useful features and another feature which can be added is to auto Schedule a task. 
   2. The design I have in mind is to create a cron expression to schedule a task. In the taskList, there would be a button to autoSchedule. Once you select the date and time
      and day, it would create a cron expression and schedule the task. The task would be linked to a job that a user selects from a list. This job would be added by the user from
      different page. Then the user would select the job when auto scheduling. The  app would connect to the job using **KAFKA**. The job would be listening on a kakfa topic.
      Once the job recieves the message on the kafka topic, it would run the job. The app would send the message to the **KAFKA**  topic at the scheduled date and time. Then the job
      would run and update the status accordingly in the task page. 
          
