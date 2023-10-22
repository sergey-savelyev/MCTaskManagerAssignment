<script>
    import { getSearchResults } from "$lib/api.js";
    import { createEventDispatcher } from 'svelte';

    let tasks = [];
    let phrase = "";
    let searchTimeout;

    const dispatch = createEventDispatcher();

    function handleRootTaskClick(taskId) {
        dispatch('rootTaskSelect', {
            taskId
        });
    }

    function handleSearchInput() {
        clearTimeout(searchTimeout);

        searchTimeout = setTimeout(async function() {
            const response = await getSearchResults(phrase);
            tasks = response.entities;
        }, 1000);
    }
</script>

<div class="modal fade" id="searchRootModal" tabindex="-1" role="dialog" aria-labelledby="rootSearchLabel" aria-hidden="true">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title" id="rootSearchLabel">Assign a root task</h5>
                <button type="button" class="close" data-bs-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>

            <div class="modal-body">
                <div class="form-group">
                    <label for="searchPhrase">Search</label>
                    <input type="text" class="form-control" placeholder="Enter search phrase" bind:value={phrase} on:input={handleSearchInput}>
                </div>
                <table class="table table-hover" id="searchResultsTable">
                    <thead>
                        <tr>
                            <th>Summary</th>
                            <th>Description</th>
                        </tr>
                    </thead>
                    <tbody>
                        {#each tasks as task}
                            <tr on:click={() => handleRootTaskClick(task.id)}>
                                <td>{task.summary}</td>
                                <td>{task.description}</td
                            ></tr>
                        {/each}
                    </tbody>
                </table>
            </div>

            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>