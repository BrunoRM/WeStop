angular.module('WeStop').factory('$hub', ['$rootScope', 'API_SETTINGS', function ($rootScope, API_SETTINGS) {
    
    let connections = [];
    function createConnection(uri) {
        let hub = new signalR.HubConnectionBuilder()
            .withUrl(API_SETTINGS.uri + uri)
            .build();

        connections.push(hub);
        return hub;
    }
    
    function on(hubConnection, eventName, sCallback) {
        hubConnection.on(eventName, data => {
            $rootScope.$apply(() => {
                if (sCallback) {
                    sCallback(data);
                }
            });
        });
    }

    function invoke(hubConnection,...args) {
        return hubConnection.invoke(...args);
    }

    function connect(hubConnection, sCallback, eCallback) {
        hubConnection.serverTimeoutInMilliseconds = 1000 * 30;
        hubConnection.onclose(function () {
            onConnectionClose();
        });

        hubConnection.start().then(function () {
            $rootScope.$apply(() => {
                if (sCallback) sCallback();
            });
        }, (e) => { 
            $rootScope.$apply(function () {
                if (eCallback) {
                    eCallback(e);
                }
            });
        });
    }

    function stop(hubConnection) {
        if (hubConnection) {
            hubConnection.connection.stop();
            hubConnection = null;
        }
    }

    function stopAllHubConnections() {
        for (let i = 0; i < connections.length; i++) {
            hubConnection = connections[i];
            if (hubConnection.connectionState === 'Connected') {
                hubConnection.connection.stop();
            }
        }

        connections = [];
    }

    return {
        createConnection: createConnection,
        on: on,
        invoke: invoke,
        connect: connect,
        stop: stop,
        stopAllHubConnections: stopAllHubConnections
    };
}]);