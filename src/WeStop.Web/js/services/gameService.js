angular.module('WeStop').factory('$game', ['$rootScope', 'API_SETTINGS', function ($rootScope, API_SETTINGS) {

    let connection = new signalR.HubConnectionBuilder()
        .withUrl(API_SETTINGS.uri + '/game')
        .withAutomaticReconnect()
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
            });
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
        if (connection.state === 'Disconnected') {
            connect(() => connection.invoke(...args));
        } else {
            connection.invoke(...args);
        }
    }

    return {
        onConnectionClose: onConnectionClose,
        on: on,
        invoke: invoke
    };

}]);