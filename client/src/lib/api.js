export async function getTasks(take, continuationToken, orderBy, descendingSort) {
    var response = await fetch('http://localhost:5006/api/tasks?take=' + take + '&continuationToken=' + continuationToken + '&orderBy=' + orderBy + '&descendingSort=' + descendingSort);

    if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
    }

    return await response.json();
}

export async function getLogs(take, continuationToken, descending) {
    var response = await fetch('http://localhost:5006/api/tasks/logs?take=' + take + '&continuationToken=' + continuationToken + '&descending=' + descending);

    if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
    }

    return await response.json();
}

export async function getTaskDetails(taskId) {
    var response = await fetch(`http://localhost:5006/api/tasks/${taskId}`);

    if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
    }

    return await response.json();
}

export async function getSearchResults(phrase) {
    var response = await fetch(`http://localhost:5006/api/tasks/search/${phrase}?continuationToken=0&take=10`);

    if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
    }

    return await response.json();
}

export async function updateTaskRoot(taskId, rootId) {
    var response = await fetch(`http://localhost:5006/api/tasks/${taskId}/root`, {
        method: 'PATCH',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ rootId })
    });

    if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
    }
}

export async function createTask(summary, priority, status, description, dueDate) {
    var response = await fetch(`http://localhost:5006/api/tasks`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ summary, priority, status, description, dueDate })
    });

    if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
    }

    return await response.json();
}

export async function updateTask(taskId, summary, priority, status, description, dueDate) {
    var response = await fetch(`http://localhost:5006/api/tasks/${taskId}`, {
        method: 'PATCH',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ summary, priority, status, description, dueDate })
    });

    if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
    }
}

export async function deleteTask(taskId) {
    var response = await fetch(`http://localhost:5006/api/tasks/${taskId}`, {
        method: 'DELETE'
    });

    if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
    }
}