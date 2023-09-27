let tasksTakeValue = 10;
let tasksSkipValue = 0;
let logsTakeValue = 10;
let logsSkipValue = 0;
let order = {
  orderBy: "",
  desc: false
};

function createTask() {
    const summary = $('#ct_summary').val();
    const priority = $('#ct_priority').val();
    const status = $('#ct_status').val();
    const description = $('#ct_description').val();
    const dueDate = $('#ct_dueDate').val();

    const taskData = {
        summary,
        priority,
        status,
        description,
        dueDate
    };

    $.ajax({
        url: 'http://localhost:5006/api/tasks',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(taskData),
        success: function() {
            $('#createTaskModal').modal('hide');
            reloadTasks();
        },
            error: function() {
            console.error(`Can't create a task`);
        }
    });
}

function reloadTasks(orderBy, desc) {
  order.desc = desc;
  order.orderBy = orderBy;
  $('#loadMoreTasksBtnContainer').show();
  $('#taskTable tbody').empty();
  tasksSkipValue = 0;
  loadTasks(orderBy, desc);
}

function loadTasks(orderBy, desc) {
    $.ajax({
        url: `http://localhost:5006/api/tasks?take=${tasksTakeValue}&skip=${tasksSkipValue}&orderBy=${orderBy ? orderBy : order.orderBy}${desc ? '&descendingSort=' + order.desc : ''}`,
        method: 'GET',
        success: function(data) {
            if (data.length < tasksTakeValue) {
                $('#loadMoreTasksBtnContainer').hide();
            }

            data.forEach(function(task) {
                $('#taskTable tbody').append(`
                <tr>
                    <td style="display: none;">${task.id}</td>
                    <td>${task.summary}</td>
                    <td>${task.priority}</td>
                    <td>${task.status}</td>
                    <td>${moment(task.createDate).format('DD.MM.yyyy')}</td>
                    <td>${moment(task.dueDate).format('DD.MM.yyyy')}</td>
                    <td>
                        <button class="btn btn-sm btn-danger delete-button" data-toggle="modal" data-target="#deleteConfirmationModal">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </td>
                </tr>
                `);
            });

            tasksSkipValue += data.length;
        },
            error: function() {
            console.error(`Can't fetch tasks from server`);
        }
    });
}

function reloadLogs() {
  $('#loadMoreLogsBtnContainer').show();

  $('#logsTable tbody').empty();
  logsSkipValue = 0;
  loadLogs();
}

function loadLogs() {
  $.ajax({
      url: `http://localhost:5006/api/tasks/logs?take=${logsTakeValue}&skip=${logsSkipValue}`,
      method: 'GET',
      success: function(data) {
          if (data.length < logsTakeValue) {
              $('#loadMoreLogsBtnContainer').hide();
          }

          data.forEach(function(log) {
              $('#logsTable tbody').append(`
              <tr>
                  <td style="display: none;">${log.id}</td>
                  <td>${log.action}</td>
                  <td>${log.timestampMsec}</td>
                  <td>${log.entityId}</td>
                  <td>${log.entityType}</td>
                  <td>${log.payload}</td>
              </tr>
              `);
          });

          logsSkipValue += data.length;
      },
          error: function() {
          console.error(`Can't fetch tasks from server`);
      }
  });
}

$('#taskTable tbody').on('click', 'tr', function() {
    const taskId = $(this).find('td:first').text();
    openTaskDetails(taskId);
  });

function clearTaskModal() {
    $('#rootTaskTable tbody').empty();

    $('#taskId').val('');
    $('#summary').val('');
    $('#description').val('');
    $('#rootId').val('');
    $('#createDate').text('');
    $('#dueDate').val('');
    $('#priority').val('');
    $('#status').val('');
  
    $('#subtasksTableBody').empty();

    $('#te_alert').hide();
}

function openTaskDetails(taskId, error) {
    clearTaskModal();

    $.ajax({
      url: `http://localhost:5006/api/tasks/${taskId}`,
      method: 'GET',
      success: function(data) {
        if (data.rootTask) {
            $('#noRootTaskMessage').hide();
            $('#assignLinkContainer').hide();
            $('#rootTaskTable').show();

            const tableRow = `
                <tr>
                <td class="hidden">${data.rootTask.id}</td>
                <td>${data.rootTask.summary}</td>
                <td>${data.rootTask.priority}</td>
                <td>${data.rootTask.status}</td>
                <td>
                  <button class="btn btn-sm btn-danger delete-button">
                      <span aria-hidden="true">&times;</span>
                  </button>
                </td>
                </tr>
            `;

            $('#rootTaskTableBody').append(tableRow);
        } else {
          $('#rootTaskTable').hide();
          $('#noRootTaskMessage').show();
          $('#assignLinkContainer').show();
        }

        $('#taskDetailsModalLabel').text(data?.summary?.substring(0,10));
  
        $('#taskId').val(data?.id);
        $('#summary').val(data?.summary);
        $('#description').val(data?.description);
        $('#rootId').val(data?.rootId);
  
        const createDateFormatted = moment(data.createDate).format('DD.MM.yyyy');

        $('#createDate').text(`Created: ${createDateFormatted}`);
        const dueDate = data?.dueDate ? new Date(data.dueDate).toISOString().split('T')[0] : '';
        $('#dueDate').val(dueDate);
  
        $('#priority').val(data?.priority);
        $('#status').val(data?.status);

        if (data.subtasks && data.subtasks.length > 0) {
            $('#noSubtasksMessage').hide();
            $('#subtasksTable').show();

            data.subtasks.forEach(subtask => {
                const row = `
                    <tr>
                    <td class="hidden">${subtask.id}</td>
                    <td>${subtask.summary}</td>
                    <td>${subtask.priority}</td>
                    <td>${subtask.status}</td>
                    </tr>
                `;
                $('#subtasksTableBody').append(row);
            });
  
        } else {
            $('#subtasksTable').hide();
            $('#noSubtasksMessage').show();
        }
  
        $('#taskDetailsModal').modal('show');

        if (error) {
            console.log(error);
            $('#te_alert').show();
            $('#te_alertText').text(error.responseText);
        }
      },
      error: function() {
        console.error('Task details retrieving error');
      }
    });
}

  $('#subtasksTable').on('click', 'tbody tr', function() {
    const taskId = $(this).find('td.hidden').text();
  
    openTaskDetails(taskId);
  });

  $('#rootTaskTable').on('click', 'tbody tr', function() {
    const taskId = $(this).find('td.hidden').text();
  
    openTaskDetails(taskId);
  });

  $('#assignLink').click(function() {
    const taskId = $('#taskId').val();
    const summary = $('#summary').val();
  
    $('#taskDetailsModal').modal('hide');
  
    $('#rootSearch').modal('show');
    $('#rootSearch').find('#assignedTaskId').val(taskId);
    $('#rootSearch').find('#assignedSummary').val(summary);
  });

$('#createTaskBtn').click(function() {
    createTask();
});

$('#createTaskModal').on('hidden.bs.modal', function () {
    $('#taskForm')[0].reset();
});

$('#taskDetailsModal').on('hidden.bs.modal', function () {
    reloadTasks();
});

$('#loadMoreTasksBtn').click(function() {
    loadTasks();
});
$('#loadMoreLogsBtn').click(function() {
  loadLogs();
});


let searchTimeout;

$('#searchPhrase').on('input', function() {
  clearTimeout(searchTimeout);

  const phrase = $(this).val();

  searchTimeout = setTimeout(function() {
    performSearch(phrase);
  }, 1000);
});

function performSearch(phrase) {
  $.ajax({
    url: `http://localhost:5006/api/tasks/search/${phrase}?take=20`,
    method: 'GET',
    success: function(data) {
      updateSearchResults(data);
    },
    error: function() {
      console.error('Search error');
    }
  });
}

function updateSearchResults(results) {
    const searchResultsTable = $('#searchResultsTable tbody');
    searchResultsTable.empty();
  
    results.forEach(function(result) {
      const summary = result.summary.substring(0, 40);
      const description = result.description.substring(0, 40);
      const row = `<tr><td class="hidden">${result.id}</td><td>${summary}</td><td>${description}</td></tr>`;
      searchResultsTable.append(row);
    });
  }

  function clearAndCloseSearchModal() {
    $('#searchPhrase').val('');
  
    $('#searchResultsTable tbody').empty();
  
    $('#rootSearch').modal('hide');
  }

  $('#searchResultsTable').on('click', 'tbody tr', function() {
    const rootId = $(this).find('td.hidden').text();
  
    $('#taskDetailsModal').find('#rootId').val(rootId);
    const taskId = $('#taskId').val()

    updateTaskRoot(taskId, rootId);
    clearAndCloseSearchModal(); 
  });

  $('#btnSaveChanges').on('click', function() {
    console.log($('#rootId').val());
    updateTask();
  });

  function updateTaskRoot(taskId, rootId) {
    const requestBody = {
        rootId: rootId
    };

    $.ajax({
        url: `http://localhost:5006/api/tasks/${taskId}/root`,
        method: 'PATCH',
        contentType: 'application/json',
        data: JSON.stringify(requestBody),
        success: function(data) {
            $('#liveToast').toast('show');

            openTaskDetails(taskId);
        },
        error: function(error) {
            console.error('Error trying to update root:', error);

            openTaskDetails(taskId, error);
        }
    });
}

function updateTask() {
    const taskId = $('#taskId').val();
    const rootId = $('#rootId').val() === '' ? null : $('#rootId').val();
    const summary = $('#summary').val();
    const priority = $('#priority').val();
    const status = $('#status').val();
    const description = $('#description').val();
    const dueDate = $('#dueDate').val();

    const requestBody = {
        id: taskId,
        rootId: rootId,
        summary: summary,
        priority: priority,
        status: status,
        description: description,
        dueDate: dueDate
    };

    $.ajax({
        url: 'http://localhost:5006/api/tasks',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(requestBody),
        success: function(data) {
            $('#liveToast').toast('show');
        },
            error: function(error) {
            console.error('Error making POST request:', error);
        }
    });
}

$('#taskTable th').on('click', function() {
    const columnName = $(this).text().trim();
    let sortOrder = $(this).data('sort-order') || 'asc';
    
    sortOrder = sortOrder === 'asc' ? 'desc' : 'asc';
    reloadTasks(columnName, sortOrder === 'desc');
    
    $('#taskTable th').removeClass('sorted-asc sorted-desc');
    $(this).addClass(sortOrder === 'asc' ? 'sorted-asc' : 'sorted-desc');
    $(this).data('sort-order', sortOrder);
  });

$('#taskTable tbody').on('click', '.delete-button', function(event) {
  event.stopPropagation();
  const taskId = $(this).closest('tr').find('td:first').text();
  $('#deleteConfirmationModal').data('task-id', taskId);
  $('#deleteConfirmationModal').modal('show');
});

$('#rootTaskTable tbody').on('click', '.delete-button', function(event) {
  event.stopPropagation();
  $('#rootId').val('');
  $(this).closest('tr').remove();
});

$('#confirmDelete').click(function() {
  const taskId = $('#deleteConfirmationModal').data('task-id');
  deleteTask(taskId);
  $('#deleteConfirmationModal').modal('hide');
});

$('#btnRefreshLogs').click(function() {
  loadLogs();
});

function deleteTask(taskId) {
  $.ajax({
      url: `http://localhost:5006/api/tasks/${taskId}`,
      method: 'DELETE',
      success: function(data) {
          reloadTasks();
      },
      error: function(error) {
          console.error('Error making POST request:', error);
      }
  });
}

$(document).ready(function() {
    loadTasks();
    loadLogs();
});