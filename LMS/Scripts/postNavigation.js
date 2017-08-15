function ActivityPostConfig(postNav, postFormObj, id) {
    document.forms[1]["PostNavigation"].value = postNav;
    document.forms[1]["PostOperation"].value = postFormObj;
    document.forms[1]["Id"].value = id;
    // Debug support
    //alert("ActivityPostConfig");
    //alert(document.forms[1]["PostNavigation"].value);
    //alert(document.forms[1]["PostOperation"].value);
    //alert(document.forms[1]["Id"].value);
}

function PostConfig(postNav, postFormObj, id, activityId) {
    document.forms[1]["PostNavigation"].value = postNav;
    document.forms[1]["PostOperation"].value = postFormObj;
    document.forms[1]["Id"].value = id;
    document.forms[1]["ActivityId"].value = activityId;
    // Debug support
    //alert("PostConfig");
    //alert(document.forms[1]["PostNavigation"].value);
    //alert(document.forms[1]["PostOperation"].value);
    //alert(document.forms[1]["Id"].value);
    //alert(document.forms[1]["ActivityId"].value);
}

function CoursePostConfig(postNav, postFormObj, id, moduleId) {
    document.forms[1]["PostNavigation"].value = postNav;
    document.forms[1]["PostOperation"].value = postFormObj;
    document.forms[1]["Id"].value = id;
    document.forms[1]["ModuleId"].value = moduleId;
    // Debug support
    //alert("CoursePostConfig");
    //alert(document.forms[1]["PostNavigation"].value);
    //alert(document.forms[1]["PostOperation"].value);
    //alert(document.forms[1]["Id"].value);
    //alert(document.forms[1]["ModuleId"].value);
}

function ClearCourseForm() {
    document.forms[1]["Name"].value = "";
    document.forms[1]["Description"].value = "";
    document.forms[1]["StartDate"].value = arguments[0];
    document.forms[1]["EndDate"].value = arguments[1];
    // Debug support
    //alert("ClearModuleForm");
    //alert(document.forms[1]["Name"].value);
    //alert(document.forms[1]["Description"].value);
    //alert(document.forms[1]["StartDate"].value);
    //alert(document.forms[1]["EndDate"].value);
}

function ClearModuleForm() {
    document.forms[1]["Name"].value = "";
    document.forms[1]["Description"].value = "";
    document.forms[1]["StartDate"].value = arguments[0];
    document.forms[1]["EndDate"].value = arguments[1];
    // Debug support
    //alert("ClearModuleForm");
    //alert(document.forms[1]["Name"].value);
    //alert(document.forms[1]["Description"].value);
    //alert(document.forms[1]["StartDate"].value);
    //alert(document.forms[1]["EndDate"].value);
}

function ClearActivityForm() {
    document.forms[1]["Name"].value = "";
    document.forms[1]["Description"].value = "";
    document.forms[1]["StartDate"].value = arguments[0];
    document.forms[1]["EndDate"].value = arguments[1];
    document.forms[1]["Deadline"].value = arguments[2];
    // Debug support
    //alert("ClearActivityForm");
    //alert(document.forms[1]["Name"].value);
    //alert(document.forms[1]["Description"].value);
    //alert(document.forms[1]["StartDate"].value);
    //alert(document.forms[1]["EndDate"].value);
    //alert(document.forms[1]["Deadline"].value);
}

function ShowCreateNewUserDiv() {
    var x = document.getElementById('CreateNewUserDiv');
    if (x.style.display === 'none') {
        x.style.display = 'block';
    } else {
        x.style.display = 'none';
    }
}