angular.module('WeStop').factory('$game', ['$rootScope', function ($rootScope) {

    let connection = new signalR.HubConnectionBuilder()
            .withUrl("http://localhost:5000/game")
            .build();

    function onConnectionClose() {
        console.log('Connection has closed');
    }

    function connect (sCallback, eCallback) {
        
        connection.serverTimeoutInMilliseconds = 1000*30;
        connection.onclose(function () {
            onConnectionClose();
        });

        connection.start().then(function () { 
            $rootScope.$apply(() => sCallback());
        }, (e) => { 
            $rootScope.$apply(function () {
                if (eCallback) {
                    eCallback(e);
                }
            })
        });
    }

    function on(eventName, sCallback) {
        connection.on(eventName, data => {
            $rootScope.$apply(() => {
                if (sCallback) {
                    sCallback(data);
                }
            });
        });
    }

    function invoke(...args) {
        if (connection.state === 0) {
            connect(() => connection.invoke(...args));
        } else {
            connection.invoke(...args);
        }
    }

    return {
        onConnectionClose: onConnectionClose,
        on: on,
        invoke: invoke
    }

}]);