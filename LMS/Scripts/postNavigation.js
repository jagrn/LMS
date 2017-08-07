function PostConfig(postNav, postFormObj, id, activityId) {
    document.forms[0]["PostNavigation"].value = postNav;
    document.forms[0]["PostOperation"].value = postFormObj;
    document.forms[0]["Id"].value = id;
    document.forms[0]["ActivityId"].value = activityId;
    // Debug support
    //alert("PostConfig");
    //alert(document.forms[0]["PostNavigation"].value);
    //alert(document.forms[0]["PostOperation"].value);
    //alert(document.forms[0]["Id"].value);
    //alert(document.forms[0]["ActivityId"].value);
}

function CoursePostConfig(postNav, postFormObj, id, moduleId) {
    document.forms[0]["PostNavigation"].value = postNav;
    document.forms[0]["PostOperation"].value = postFormObj;
    document.forms[0]["Id"].value = id;
    document.forms[0]["ModuleId"].value = moduleId;
    // Debug support
    //alert("PostConfig");
    //alert(document.forms[0]["PostNavigation"].value);
    //alert(document.forms[0]["PostOperation"].value);
    //alert(document.forms[0]["Id"].value);
    //alert(document.forms[0]["ActivityId"].value);
}

function ClearCourseForm() {
    document.forms[0]["Name"].value = "";
    document.forms[0]["Description"].value = "";
    document.forms[0]["StartDate"].value = arguments[0];
    document.forms[0]["EndDate"].value = arguments[1];
    // Debug support
    //alert("ClearModuleForm");
    //alert(document.forms[0]["Name"].value);
    //alert(document.forms[0]["Description"].value);
    //alert(document.forms[0]["StartDate"].value);
    //alert(document.forms[0]["EndDate"].value);
}

function ClearModuleForm() {
    document.forms[0]["Name"].value = "";
    document.forms[0]["Description"].value = "";
    document.forms[0]["StartDate"].value = arguments[0];
    document.forms[0]["EndDate"].value = arguments[1];
    // Debug support
    //alert("ClearModuleForm");
    //alert(document.forms[0]["Name"].value);
    //alert(document.forms[0]["Description"].value);
    //alert(document.forms[0]["StartDate"].value);
    //alert(document.forms[0]["EndDate"].value);
}

function ClearActivityForm() {
    document.forms[0]["Name"].value = "";
    document.forms[0]["Description"].value = "";
    document.forms[0]["StartDate"].value = arguments[0];
    document.forms[0]["EndDate"].value = arguments[1];
    document.forms[0]["Deadline"].value = arguments[2];
    // Debug support
    //alert("ClearActivityForm");
    //alert(document.forms[0]["Name"].value);
    //alert(document.forms[0]["Description"].value);
    //alert(document.forms[0]["StartDate"].value);
    //alert(document.forms[0]["EndDate"].value);
    //alert(document.forms[0]["Deadline"].value);
}

