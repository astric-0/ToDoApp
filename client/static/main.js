const Ajax = (path = '', options = {}) => $.ajax({
    method: 'GET',
    url: 'http://localhost:5283' + path,
    ...options
});

let allTasks = [];

function setAllTasks (tasks) {
    allTasks = tasks;
    render(tasks);
}

function render(tasks) {
    const tasksRows = $("#tasksRow");
    if (tasksRows) {

        let elements = '';
        tasks.forEach((element, index) => {
            elements +=  taskTemplate(element, index);
        });
        
        tasksRows.append(elements);
    }
    else 
        console.log("null");
}

function deleteTask (id, index) {    
    Ajax(
        '/tasks/deletetask/' + id,
        {
            method: 'delete',
            success: _ => {
                $("#task"+id).remove();
                allTasks.splice(index, 1);
                console.log(allTasks);
            }, 
            error: (jqXHR, exception) => {
                console.log(jqXHR.status);
                console.log(jqXHR.responseText);
            }
        }
    );
}

function toggleCompleted (id, index) {
    Ajax(
        '/tasks/tooglecomplete/' + id,
        {
            method: 'PUT',
            success: _ => {
                const isChecked = $("#taskCheck" + id)
                .prop("checked", !allTasks[index].isCompleted)
                .is(":checked");

                allTasks[index].isCompleted = !allTasks[index].isCompleted;

                console.log("checked: ", isChecked);

                const taskBox = $("#task" + id + " .card");                
                taskBox
                .removeClass("border-success border-primary")
                .addClass(isChecked ? "border-success" : "border-primary");

                const taskDate = $("#taskDate" + id);
                const taskImportance = allTasks[index].importance;
                taskDate
                    .removeClass("text-success text-primary text-danger")
                    .addClass(
                        isChecked
                        ? "text-success"
                        : taskImportance == 1 
                            ? 'text-danger'
                            : taskImportance == 2
                                ? 'text-primary'
                                : 'text-success'
                    );
                
            },
            error: (jqXHR, exception) => {
                console.log(jqXHR.status);
                console.log(jqXHR.responseText);
                $("#taskCheck" + id).prop("checked", allTasks[index].isCompleted);
            }
        }
    )
}

function taskTemplate (task, index, isUpdate = false) {
    const { id, taskName, taskDetails, createdOn, modifiedOn, completedOn, deadline, importance, category, isCompleted } = task;
    
    const categoryIcon = category == 'OFFICE' 
    ? 'fa-briefcase' 
    : category == 'HOME'
        ? 'fa-house'        
        : category == 'MARKET'
            ? 'fa-shop'
            : 'fa-face-smile'
    ;    

    const cardClass = isCompleted == true
    ? 'success'
    : 'primary'
    ;

    const importanceClass = isCompleted == true
    ? 'success'
    :
        importance == 1
        ? 'danger'
        : importance == 2
            ? 'primary'
            : 'success';

    return `
    ${ isUpdate ? '' : `<div class="col-md-4 my-3" id=${'task' + id}>` }
            <div class="card shadow border-1 border-${ cardClass }">
                <div class="card-header border-0 bg-white">
                    <div class="row justify-content-between">
                        <div class="col">
                            <h3 class="card-title text-primary"><i class="fa-solid ${ categoryIcon } text-primary"></i> ${ taskName }</h3>
                        </div>

                        <div class="col-2">
                            <div class="form-check form-switch mt-1">
                                <input class="form-check-input" id=${"taskCheck" + id} role="switch" type="checkbox" title="Mark Completed" onclick="toggleCompleted(${id}, ${index})" ${ isCompleted ? 'checked' : '' }>
                            </div>
                        </div>
                    </div>
                </div>

                ${
                    taskDetails?.length > 0
                    ?
                    `<div class="card-body">
                        <div class="card-text"><b>${ taskDetails }</b></div>
                    </div>`
                    :
                    ''
                }

                <div class="card-footer bg-white border-0">
                    <div class="row justify-content-between">

                        <div class="col-6">
                            <span class="btn text-${importanceClass} px-0" id=${'taskDate' + id}  >
                                ${ 
                                    completedOn
                                    ? '<i class="fa-solid fa-check"></i>'
                                    :
                                        deadline 
                                        ? '<i class="fa-solid fa-hourglass-end"></i>' 
                                        : modifiedOn
                                            ? '<i class="fa-solid fa-wrench"></i>'
                                            : '<i class="fa-solid fa-calendar "></i>' 
                                }
                                ${ new Date(completedOn ?? deadline ?? modifiedOn ?? createdOn).toDateString() }
                            </span>
                        </div>

                        <div class="col-3">
                            <div class="btn-group" role="group"> 
                                <button class="btn text-primary px-2" data-bs-toggle="modal" data-bs-target="#addTaskModal" onclick="handleEditClick(${id}, ${index})">
                                    <i class="fa-solid fa-pen-to-square fa-lg"></i>
                                </button>

                                <button class="btn text-danger px-2" onclick="deleteTask(${id}, ${index});">
                                    <i class="fa-solid fa-trash fa-lg"></i>
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>  
        ${ isUpdate ? '' : '</div>' }
    `;
} 

function loadPage (page) {
    const contentArea = $("#contentArea");
    contentArea.load('views/' + page, _ => {
        if (page == 'tasks.html')
            render(allTasks);
    });
}

function handleChooseTaskType (type) {
    $(".addTaskBtnGroup").removeClass("active");
    $("#btn"+type).addClass("active");
}

function handleTaskImportance () {
    const props = { 
        1: {
            text: "High",
            className: "text-danger"
        },
        2: {
            text: "Medium",
            className: "text-primary"
        },
        3: {
            text: "Low",
            className: "text-success"
        }
    };

    const val = $("input[name=importance]").val();

    $("#taskImportanceLabel")
    .text(props[val].text)
    .removeClass("text-danger text-primary text-success")
    .addClass(props[val].className);
}

function resetModalFields () {
    $(".modal-title").text('Add task');
    $("input[name='taskName']").val(''),
    $("textarea[name='taskDetails']").val(''),
    $("input[name='deadline']").val(''),
    $("input[name='importance']").val(2);
    $(".addTaskBtnGroup").removeClass("active");
    $("#btnHome").addClass("active");
    handleTaskImportance();
    $("#submitBtn").attr({ "onclick": 'handleAddTask()', "disabled": true });
}

function handleAddTask (id, index) {

    const isUpdate = id != undefined && index != undefined ? { id, index } : false;

    const payload = {
        ...allTasks[index],
        taskName: $("input[name='taskName']").val(),
        taskDetails: $("textarea[name='taskDetails']").val(),
        deadlineDateString: $("input[name='deadline']").val(),
        importance: $("input[name='importance']").val(),        
    };
    
    if (!payload.taskName || payload.taskName.length == 0)
        return;

    delete payload.deadline;

    payload.category = 'Home';
    $(".addTaskBtnGroup").each((i, element) => {
        if(element.classList.contains("active"))
            payload.category = element.value.toUpperCase();
    });     

    Ajax(
        isUpdate ? "/tasks/updatetask" : "/tasks/addtask",
        {
            method: isUpdate ? 'PUT' : 'POST',
            data: JSON.stringify(payload),            
            contentType: 'application/json',

            success: response => {
                if (payload.deadlineDateString)
                    payload.deadline = new Date(payload.deadlineDateString).toDateString();

                if (!isUpdate) {                
                    payload.id = response;
                    payload.createdOn = new Date().toDateString();                    

                    allTasks.push(payload);
                    $("#tasksRow").append(taskTemplate(payload, allTasks.length - 1, false));
                }
                else {
                    allTasks[isUpdate.index] = payload;
                    $("#task" + isUpdate.id).html(taskTemplate(payload, isUpdate.index, true));
                }
            },

            error: (jqXHR, exception) => {
                console.log("error: ", exception);
                console.log(jqXHR.status);
                console.log(jqXHR.responseText);
            }        
        }
    );   
}

function handleEditClick (id, index) {
    resetModalFields();

    const { taskName, taskDetails, deadline, category, importance, createdOn, modifiedOn, completedOn } = allTasks[index];
    const modal = $("#addTaskModal")
    
    modal.find(".modal-title").text("Update task");    
    modal.find("input[name='taskName']").val(taskName);
    modal.find("textarea[name='taskDetails']").val(taskDetails);

    deadline && modal.find("input[name='deadline']").val(new Date().toISOString().split("T")[0]);

    modal.find(".addTaskBtnGroup").each((i, element) => {
        if(element.value.toUpperCase() == category.toUpperCase())        
            $(element).addClass('active');
        else 
            $(element).removeClass('active');
    });

    modal.find("input[name='importance']").val(importance);
    handleTaskImportance();
    
    $("#submitBtn").removeAttr('onclick');
    $("#submitBtn").attr({ "onclick": `handleAddTask(${id}, ${index})`, "disabled": false });
}

$(document).ready(async _ => {
    loadPage('tasks.html');

    Ajax("/tasks/getalltasks")
    .done(allTasksResponse => {        
        setAllTasks(allTasksResponse);
    });
});