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

    API.createTask(summary, priority, status, description, dueDate)
      .done(function(data) {
        $('#createTaskModal').modal('hide');
        reloadTasks();
      })
      .fail(function(error) {
      });
}

function reloadTasks(orderBy, desc) {
  order.desc = desc;
  order.orderBy = orderBy;
  $('#loadMoreTasksBtnContainer').show();
  $('#taskTable tbody').empty();
  tasksSkipValue = null;
  loadTasks(orderBy, desc);
}

function loadTasks(orderBy, desc) {
    $.ajax({
        url: `http://localhost:5006/api/tasks?take=${tasksTakeValue}${tasksSkipValue ? '&continuationToken=' + tasksSkipValue : ''}&orderBy=${orderBy ? orderBy : order.orderBy}${desc ? '&descendingSort=' + order.desc : ''}`,
        method: 'GET',
        success: function(data) {
            if (data.length < tasksTakeValue) {
                $('#loadMoreTasksBtnContainer').hide();
            }

            data.entities.forEach(function(task) {
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

            tasksSkipValue = data.continuationToken;
        },
            error: function() {
            console.error(`Can't fetch tasks from server`);
        }
    });
}

function reloadLogs() {
  $('#loadMoreLogsBtnContainer').show();

  $('#logsTable tbody').empty();
  logsSkipValue = null;
  loadLogs();
}

function loadLogs() {
  $.ajax({
      url: `http://localhost:5006/api/tasks/logs?take=${logsTakeValue}${logsSkipValue ? '&continuationToken=' + logsSkipValue : ''}&descending=false`,
      method: 'GET',
      success: function(data) {
          if (data.entities.length < logsTakeValue) {
              $('#loadMoreLogsBtnContainer').hide();
          }

          data.entities.forEach(function(log) {
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

          logsSkipValue = data.continuationToken;
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
    API.getTaskDetails(taskId)
      .done(function(data) {
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

        $('#taskDetailsModal').data('task-id', data?.id);
        $('#taskDetailsModal').data('root-id', data?.rootId);

        $('#summary').val(data?.summary);
        $('#description').val(data?.description);

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
            console.error(error);
            $('#te_alert').show();
            $('#te_alertText').text(error.responseText);
        }
      })
      .fail(function(error) {
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
  $('#rootSearch').data('task-id', taskId);
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
  API.getSearchResults(phrase)
    .done(function(data) {
      updateSearchResults(data.entities);
    })
    .fail(function(error) {
      console.error('Search error', error);
    });
}

function updateSearchResults(results) {
  const searchResultsTable = $('#searchResultsTable tbody');
  searchResultsTable.empty();

  results.forEach(function(result) {
    const summary = result.summary.substring(0, 40);
    const description = result.description?.substring(0, 40) ?? "";
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
  const taskId = $('#taskDetailsModal').data('task-id');

  updateTaskRoot(taskId, rootId);
  clearAndCloseSearchModal(); 
});

$('#btnSaveChanges').on('click', function() {
  updateTask();
});

function updateTaskRoot(taskId, rootId) {
  const requestBody = {
      rootId: rootId
  };

  API.updateTaskRoot(taskId, rootId)
    .done(function(data) {
      $('#liveToast').toast('show');

      openTaskDetails(taskId);
    })
    .fail(function(error) {
      console.error('Error making PATCH request:', error);

      openTaskDetails(taskId, error);
    });
}

function updateTask() {
    const taskId = $('#taskDetailsModal').data('task-id');
    const rootId = $('#taskDetailsModal').data('root-id') === '' ? null : $('#taskDetailsModal').data('root-id');
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

    API.updateTask(taskId, summary, priority, status, description, dueDate)
      .done(function(data) {
        $('#te_alertSuccessText').text('Successfully updated');
        $('#te_alertSuccess').show();
      })
      .fail(function(error) {
        $('#te_alertText').text(error.responseText);
        $('#te_alert').show();
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
  $('#taskDetailsModal').data('root-id', null);
  API.updateTaskRoot($('#taskDetailsModal').data('task-id'), null)
    .done(function(data) {
      $('#rootTaskTable tbody tr').remove();
    })
    .fail(function(error) {
      $("#te_alertText").text(error.responseText);
      $("#te_alert").show();
    });
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
  API.deleteTask(taskId)
    .done(function(data) {
      reloadTasks();
    })
    .fail(function(error) {
      console.error('Error making DELETE request:', error);
    });
}

$(document).ready(function() {
    loadTasks();
    loadLogs();
});

const API = {
  getTasks: function() {
    return $.ajax({
      url: `http://localhost:5006/api/tasks`,
      method: 'GET'
    });
  },
  getLogs: function() {
    return $.ajax({
      url: `http://localhost:5006/api/tasks/logs`,
      method: 'GET'
    });
  },
  getTaskDetails: function(taskId) {
    return $.ajax({
      url: `http://localhost:5006/api/tasks/${taskId}`,
      method: 'GET'
    });
  },
  getSearchResults: function(phrase) {
    return $.ajax({
      url: `http://localhost:5006/api/tasks/search/${phrase}?continuationToken=0&take=10`,
      method: 'GET'
    });
  },
  updateTaskRoot: function(taskId, rootId) {
    return $.ajax({
      url: `http://localhost:5006/api/tasks/${taskId}/root`,
      method: 'PATCH',
      contentType: 'application/json',
      data: JSON.stringify({ rootId })
    });
  },
  createTask: function(summary, priority, status, description, dueDate) {
    return $.ajax({
      url: `http://localhost:5006/api/tasks`,
      method: 'POST',
      contentType: 'application/json',
      data: JSON.stringify({ summary, priority, status, description, dueDate })
    });
  },
  updateTask: function(taskId, summary, priority, status, description, dueDate) {
    return $.ajax({
      url: `http://localhost:5006/api/tasks/${taskId}`,
      method: 'PATCH',
      contentType: 'application/json',
      data: JSON.stringify({ summary, priority, status, description, dueDate })
    });
  },
  deleteTask: function(taskId) {
    return $.ajax({
      url: `http://localhost:5006/api/tasks/${taskId}`,
      method: 'DELETE'
    });
  }
};