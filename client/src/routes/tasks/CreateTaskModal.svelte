<script>
    import { createEventDispatcher } from 'svelte';

    let task = {
        summary: "",
        priority: "Low",
        status: "Reserved",
        description: "",
        dueDate: Date.now()
    };

    const dispatch = createEventDispatcher();

    function handleCreateClick() {
        dispatch('create', {
            task
        });
    }
</script>

<div class="modal fade" id="createTaskModal" tabindex="-1" role="dialog" aria-labelledby="createTaskModalLabel" aria-hidden="true">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title" id="createTaskModalLabel">Create Task</h5>
                <button type="button" class="close" data-bs-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>

            <div class="modal-body">
                <form id="taskForm">
                    <div class="form-group">
                        <label for="summary">Summary</label>
                        <input type="text" class="form-control" maxlength="50" bind:value={task.summary}>
                    </div>
                    <div class="form-group">
                        <label for="priority">Priority</label>
                        <select class="form-control" bind:value={task.priority}>
                            <option value="Low">Low</option>
                            <option value="Normal">Normal</option>
                            <option value="High">High</option>
                            <option value="Urgent">Urgent</option>
                        </select>
                    </div>
                    <div class="form-group">
                        <label for="status">Status</label>
                        <select class="form-control" bind:value={task.status}>
                            <option value="Reserved">Reserved</option>
                            <option value="Ongoing">Ongoing</option>
                            <option value="Done">Done</option>
                            <option value="Pending">Pending</option>
                        </select>
                    </div>
                    <div class="form-group">
                        <label for="description">Description</label>
                        <input type="text" class="form-control" bind:value={task.description}>
                    </div>
                    <div class="form-group">
                        <label for="dueDate">Due Date</label>
                        <input type="date" class="form-control" bind:value={task.dueDate}>
                    </div>
                </form>
            </div>
            <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
            <button type="button" class="btn btn-primary" data-bs-dismiss="modal" on:click={handleCreateClick}>Create</button>
            </div>
        </div>
    </div>
</div>