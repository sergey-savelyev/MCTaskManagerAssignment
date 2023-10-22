<script>
    import { createEventDispatcher } from 'svelte';

    export let task;

    const dispatch = createEventDispatcher();
    let showConfirmation = false;

    function removeTask(event) {
        event.stopPropagation();
        showConfirmation = false;
        dispatch('delete', { id : task.id });
    }
</script>

<tr>
    <td on:click>{task.summary}</td>
    <td on:click>{task.priority}</td>
    <td on:click>{task.status}</td>
    <td on:click>{new Date(Date.parse(task.createDate)).toLocaleString('en-GB')}</td>
    <td on:click>{new Date(Date.parse(task.dueDate)).toLocaleDateString('en-GB')}</td>
    <td>
        {#if !showConfirmation}
            <button class="btn btn-sm btn-danger delete-button" on:click={() => showConfirmation = true}>
                <span aria-hidden="true">&times;</span>
            </button>
        {:else}
            <p>Delete the task?</p>
            <button class="btn btn-sm btn-danger" on:click={removeTask}>
                yes
            </button>
            <button class="btn btn-sm btn-secondary" on:click={() => showConfirmation = false}>
                no
            </button>
        {/if}
    </td>
</tr>