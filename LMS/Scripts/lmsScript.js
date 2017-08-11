$(document).ready(
          function () {
              $('#priorWeek').click(function () {
                  //alert("They are here");
                  //$('#schedule').toggle(500, function () {
                    //  $('#schedule').load('@Url.Action("Students","SchemePartial")');
                  //});
                  $.ajax("SchemePartial")
              });
          });

//,  { courseId :1, year : Model.Year, week :Model.Week, moveWeek :-1 }
