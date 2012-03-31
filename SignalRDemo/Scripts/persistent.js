$(function () {
    var lastMessage, connection = $.connection('echo');
    connection.received(function (data) {
        var timeDif, receiveTime;
        receiveTime = new Date();

        timeDif = receiveTime.getTime() - lastMessage.getTime();
        lastMessage = receiveTime;
        $('#messages').html(timeDif);
        $("#messageCount").html(data);
    });

    lastMessage = new Date();
    connection.start();
    $("#broadcast").click(function () {
        connection.send($('#msg').val());
    });
});