function ActivityPostConfig(postNav, postFormObj, id, showDocuments) {   
    document.forms[1]["PostNavigation"].value = postNav;
    document.forms[1]["PostOperation"].value = postFormObj;
    document.forms[1]["Id"].value = id;
    document.forms[1]["ShowDocuments"].value = showDocuments;
    // Debug support
    //alert("ActivityPostConfig");
    //alert(document.forms[1]["PostNavigation"].value);
    //alert(document.forms[1]["PostOperation"].value);
    //alert(document.forms[1]["Id"].value);
}

function PostConfig(postNav, postFormObj, id, activityId, showActivities, showDocuments) {    
    document.forms[1]["PostNavigation"].value = postNav;
    document.forms[1]["PostOperation"].value = postFormObj;
    document.forms[1]["Id"].value = id;
    document.forms[1]["ActivityId"].value = activityId;
    document.forms[1]["ShowActivities"].value = showActivities;
    document.forms[1]["ShowDocuments"].value = showDocuments;
    // Debug support
    //alert("ModulePostConfig");
    //alert(document.forms[1]["PostNavigation"].value);
    //alert(document.forms[1]["PostOperation"].value);
    //alert(document.forms[1]["Id"].value);
    //alert(document.forms[1]["ActivityId"].value);
}

function CoursePostConfig(postNav, postFormObj, id, moduleId, showModules, showDocuments) {  
    document.forms[1]["PostNavigation"].value = postNav;
    document.forms[1]["PostOperation"].value = postFormObj;
    document.forms[1]["Id"].value = id;
    document.forms[1]["ModuleId"].value = moduleId;
    document.forms[1]["ShowModules"].value = showModules;
    document.forms[1]["ShowDocuments"].value = showDocuments;
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

// Select all links with hashes
$('a[href*="#"]')
  // Remove links that don't actually link to anything
  .not('[href="#"]')
  .not('[href="#0"]')
  .click(function (event) {
      // On-page links
      if (
        location.pathname.replace(/^\//, '') == this.pathname.replace(/^\//, '')
        &&
        location.hostname == this.hostname
      ) {
          // Figure out element to scroll to
          var target = $(this.hash);
          target = target.length ? target : $('[name=' + this.hash.slice(1) + ']');
          // Does a scroll target exist?
          if (target.length) {
              // Only prevent default if animation is actually gonna happen
              event.preventDefault();
              $('html, body').animate({
                  scrollTop: target.offset().top
              }, 1000, function () {
                  // Callback after animation
                  // Must change focus!
                  var $target = $(target);
                  $target.focus();
                  if ($target.is(":focus")) { // Checking if the target was focused
                      return false;
                  } else {
                      $target.attr('tabindex', '-1'); // Adding tabindex for elements not focusable
                      $target.focus(); // Set focus again
                  };
              });
          }
      }
  });