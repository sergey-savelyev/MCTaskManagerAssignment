<script>
    import { getTaskDetails, updateTaskRoot, updateTask } from "$lib/api.js";
    import { page } from '$app/stores';
    import { onMount } from 'svelte';
    import SearchRootModal from "./SearchRootModal.svelte";
    import { goto } from "$app/navigation";

    let loading = true;
    let task;
    let message;
    let dueDate;

    onMount(async () => {
        task = await getTaskDetails($page.params.taskId);
        dueDate = task.dueDate.substring(0, task.dueDate.indexOf('T'));
        loading = false;
    });

    function handleAssignRootTaskClick() {
        const searchRootModal = new bootstrap.Modal('#searchRootModal');
        searchRootModal.show();
    }

    async function onRootTaskSelected(event) {
        await updateTaskRoot(task.id, event.detail.taskId);
        task = await loadTask();
    }

    async function handleRemoveRootTaskClick() {
        await updateTaskRoot(task.id, null);
        task = await loadTask();
    }

    async function handleSaveChangesClick() {
        task.dueDate = new Date(Date.parse(dueDate)).toISOString();
        await updateTask(task.id, task.summary, task.priority, task.status, task.description, task.dueDate);
        task = await loadTask();
    }

    async function loadTask() {
        loading = true;
        const result = await getTaskDetails($page.params.taskId);
        loading = false;

        dueDate = task.dueDate.substring(0, task.dueDate.indexOf('T'));

        return result;
    }

    async function navigateToTask(id) {
        await goto(id);
        task = await loadTask();
    }
</script>

<div class="row">
    <div class="col-2"></div>
    <div class="col-8 justify-text-center">
        {#if loading}
            <div class="spinner-grow text-primary" role="status">
                <span class="visually-hidden">Loading...</span>
            </div>
        {:else}

        {#if message?.success}
            <div class="alert alert-success alert-dismissible fade show" role="alert">
                <p>{message.text}</p>
                <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
        {:else if message?.success === false}
            <div class="alert alert-danger alert-dismissible fade show" role="alert">
                <p>{message.text}</p>
                <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
        {/if}

        <h5 class="modal-title" id="taskDetailsModalLabel">
            {task.summary}
        </h5>
        <small class="text-muted float-right ml-2" id="createDate">Created: {new Date(Date.parse(task.createDate)).toLocaleString('en-GB')}</small>


        <form id="taskDetailsForm">
            <div class="form-group">
                <label for="RootTask">Root Task</label>

                {#if !task.rootTask}
                    <div class="text-secondary text-center my-2">No root task</div>
                    <div class="text-center my-2" id="assignLinkContainer">
                        <a href="#" on:click={handleAssignRootTaskClick}>assign?</a>
                    </div>
                {:else}
                    <table class="table table-hover">
                        <thead>
                            <tr>
                                <th scope="col">Summary</th>
                                <th scope="col">Priority</th>
                                <th scope="col">Status</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td on:click={async () => await navigateToTask(task.rootTask.id)}>{task.rootTask.summary}</td>
                                <td on:click={async () => await navigateToTask(task.rootTask.id)}>{task.rootTask.priority}</td>
                                <td on:click={async () => await navigateToTask(task.rootTask.id)}>{task.rootTask.status}</td>
                                <td>
                                    <button class="btn btn-sm btn-danger delete-button" on:click={handleRemoveRootTaskClick}>
                                        <span aria-hidden="true">&times;</span>
                                    </button>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                {/if}
            </div>

            <div class="form-group">
                <label for="summary">Summary</label>
                <input type="text" class="form-control" bind:value={task.summary}>
            </div>
            <div class="form-group row">
                <div class="col">
                    <label for="priority">Priority</label>
                    <select class="form-control" bind:value={task.priority}>
                        <option value="Low">Low</option>
                        <option value="Normal">Normal</option>
                        <option value="High">High</option>
                        <option value="Urgent">Urgent</option>
                    </select>
                </div>
                <div class="col">
                    <label for="status">Status</label>
                    <select class="form-control" bind:value={task.status}>
                        <option value="Reserved">Reserved</option>
                        <option value="Ongoing">Ongoing</option>
                        <option value="Done">Done</option>
                        <option value="Pending">Pending</option>
                    </select>
                </div>
            </div>

            <div class="form-group">
                <label for="dueDate">Due Date</label>
                <input type="date" class="form-control" bind:value={dueDate}>
            </div>

            <div class="form-group">
                <label for="description">Description</label>
                <textarea class="form-control" rows="5" bind:value={task.description}></textarea>
            </div>

            <div class="form-group">
                <label for="Subtasks">Subtasks</label>
                {#if !task.subtasks}
                    <div class="text-secondary text-center my-2">No subtasks</div>
                {:else}
                    <table class="table table-hover">
                        <thead>
                            <tr>
                                <th scope="col">Summary</th>
                                <th scope="col">Priority</th>
                                <th scope="col">Status</th>
                            </tr>
                        </thead>
                        <tbody>
                            {#each task.subtasks as subtask}
                                <tr on:click={async () => await navigateToTask(subtask.id)}>
                                    <td>{subtask.summary}</td>
                                    <td>{subtask.priority}</td>
                                    <td>{subtask.status}</td>
                                </tr>
                            {/each}
                        </tbody>
                    </table>
                {/if}
            </div>
        </form>

        <button type="button" class="btn btn-primary" on:click={handleSaveChangesClick}>Save changes</button>
        <button type="button" class="btn btn-secondary" on:click={() => goto('/tasks')}>Back to the list</button>

        <SearchRootModal on:rootTaskSelect={onRootTaskSelected} />
        {/if}
    </div>
    <div class="col-2"></div>
</div>