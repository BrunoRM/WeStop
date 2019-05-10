angular.module('WeStop').factory('$game', ['$rootScope', function ($rootScope) {

    var connection = null;

    function onConnectionClose() {
        console.log('Connection has closed');
    }

    function connect (sCallback, eCallback) {

        if (connection) {
            sCallback();
        }

        connection = new signalR.HubConnectionBuilder()
            .withUrl("http://localhost:5000/gameroom")
            .build();

        connection.onclose(function () {
            onConnectionClose();
        });

        connection.start().then(() => { 
            $rootScope.$apply(() => sCallback());
        }, (e) => { 
            $rootScope.$apply(() => eCallback(e));
        });
    }

    function on(eventName, sCallback, eCallback) {

        if (!connection) {
            connect(function () {
                connection.on(eventName, data => {
                    $rootScope.$apply(() => {
                        sCallback(data);
                    });
                });        
            });
        } else {
            connection.on(eventName, data => {
                $rootScope.$apply(() => {
                    sCallback(data);
                });
            });
        }        
    }

    function invoke(eventName, params) {

        if (!connection) {
            connect(() => {
                if (params)
                    connection.invoke(eventName, params);
                else
                    connection.invoke(eventName);
            });
        } else {
            if (params)
                connection.invoke(eventName, params);
            else
                connection.invoke(eventName);
        }

    }

    return {
        onConnectionClose: onConnectionClose,
        connect: connect,
        on: on,
        invoke: invoke
    }


}]);