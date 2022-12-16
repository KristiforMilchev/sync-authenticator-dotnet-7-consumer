

//initializing CalendarApp
$(window).on('load', function() {

    StartCalendar();

});


var Contracts;
function StartCalendar() {


    $.ajax({
        method: "GET",
        contentType: "application/json",
        url: "/Home/GetUserContracts",
    }).done(async function (msg) {
        if (msg === null) {
            toastr.warning('Please connect your wallet.');
            return;
        }
        Contracts = msg;
        InitContractgrid(msg,null);
    });

    $.ajax({
        method: "GET",
        contentType: "application/json",
        url: "/Home/GetUserCompletedContracts",
    }).done(async function (msg) {
        if (msg === null) {
            toastr.warning('Please connect your wallet.');
            return;
        }

        InitCompleted(msg);
    });
}


function FilterSignatures() {
    InitContractgrid(Contracts, 1);
}

function FilterActive() {
    InitContractgrid(Contracts, 2);

}

function InitContractgrid(data, filter) {

    var bindingData = [];
    var i = 0;
    var gridData = "";

    for (var elem in data) {
        var currentStatus;
        var current = data[elem];


        if (filter !== null && current.status != filter) {

        }
        else {
            if (current.status == 1) {
                bindingData[i] = {
                    title: current.title,
                    start: current.startDate,
                    end: current.endDate,
                    className: 'bg-inactive',
                    _id: current.id
                };
                currentStatus = `<td id="Status_` + current.id + `"><span style="color:white !important" class="badge bg-light-inactive text-inactive fw-normal">Not Signed</span></td>`;
            }
            else if (current.status == 2) {
                bindingData[i] = {
                    title: current.title,
                    start: current.startDate,
                    end: current.endDate,
                    className: 'bg-success',
                    _id: current.id
                };
                currentStatus = `<td id="Status_` + current.id + `"><span class="badge bg-light-success text-success fw-normal">Active</span></td>`;
            }
            else if (current.status == 3) {
                bindingData[i] = {
                    title: current.title,
                    start: current.startDate,
                    end: current.endDate,
                    className: 'bg-info',
                    _id: current.id
                };
                currentStatus = `<td id="Status_` + current.id + `"><span class="badge bg-light-warning text-warning fw-normal">Requested Signature</span></td>`;
            }
            else if (current.status == 4) {
                bindingData[i] = {
                    title: current.title,
                    start: current.startDate,
                    end: current.endDate,
                    className: 'bg-danger',
                    _id: current.id

                };
                currentStatus = `<td id="Status_` + current.id + `"><span class="badge bg-light-info text-info fw-normal">Requesting Payment</span></td>`;
            }
            else if (current.status == 5) {
                bindingData[i] = {
                    title: current.title,
                    start: current.startDate,
                    end: current.endDate,
                    className: 'bg-danger',
                    _id: current.id

                };
                currentStatus = `<td id="Status_` + current.id + `"><span class="badge bg-light-success text-success fw-normal">Complete</span></td>`;
            }
            else if (current.status == 6) {
                bindingData[i] = {
                    title: current.title,
                    start: current.startDate,
                    end: current.endDate,
                    className: 'bg-danger',
                    _id: current.id

                };
                currentStatus = `<td id="Status_` + current.id + `" ><span class="badge bg-light-success text-success fw-normal">Confirming Payment</span></td>`;
            }
            else {
                bindingData[i] = {
                    title: current.title,
                    start: current.startDate,
                    end: current.endDate,
                    className: 'bg-danger',
                    _id: current.id

                };
                currentStatus = `<td id="Status_` + current.id + `" ><span class="badge bg-light-success text-success fw-normal">Completed</span></td>`;
            }
            var cProvider = current.serviceProider;
            if (cProvider == null)
                cProvider = "--";
            else {
                var lenght = cProvider.length;
                cProvider = cProvider.substring(0, 3) + "..." + cProvider.substring(lenght - 3, lenght);
            }

            gridData += `  <tr class="tableRowSelectable" onclick="GetEventData(` + current.id + `)" id="id_` + current.id + `">
                                    <td style="justify-content: left;">
                                       ` + current.title + `
                                    </td>
                                    <td>`+ cProvider + `</td>
                                     
                                    <td class="hideMobile">
                                        `+ new Date(current.endDate).toLocaleDateString("en-us") + `
                                    </td>
                                    `+ currentStatus + `

                                </tr>`;

            i++;
        }
    }

    $("#gridBody").html(gridData);
    if (gridData === "") {
        $("#emptyGrid").show();
    }
    else {
        $("#emptyGrid").hide();

    }

}


function InitCompleted(data) {
    var i = 0;
    var gridData = "";
    var bindingData = [];

    for (var elem in data) {
        var currentStatus;
        var current = data[elem];

        
        if (current.status >= 6) {
            bindingData[i] = {
                title: current.title,
                start: current.startDate,
                end: current.endDate,
                className: 'bg-danger',
                _id: current.id

            };
            currentStatus = `<td id="Status_` + current.id + `"><span class="badge bg-light-success text-success fw-normal">Complete</span></td>`;
        }
        var cProvider = current.serviceProider;
        if (cProvider == null)
            cProvider = "--";
        else {
            var lenght = cProvider.length;
            cProvider = cProvider.substring(0, 3) + "..." + cProvider.substring(lenght - 3, lenght);
        }

        gridData += `  <tr class="tableRowSelectable" onclick="GetEventData(` + current.id + `)" id="id_` + current.id + `">
                        <td style="justify-content: left;">
                            ` + current.title + `
                        </td>
                        <td>`+ cProvider + `</td>
                                     
                        <td>
                            `+ new Date(current.endDate).toLocaleDateString("en-us") + `
                        </td>
                        `+ currentStatus + `
                    </tr>`;

        i++;
    }

    $("#gridBodyComplete").html(gridData);
}