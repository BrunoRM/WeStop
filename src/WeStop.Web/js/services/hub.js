angular.module('WeStop').factory('$hub', ['$rootScope', 'API_SETTINGS', function ($rootScope, API_SETTINGS) {
    let uri = '';
    let hubConnection = createNewHubConnection();

    function onConnectionClose() {
        console.log('hubConnection has closed');
    }

    function connect (sCallback, eCallback) {
        hubConnection.serverTimeoutInMilliseconds = 1000 * 30;
        hubConnection.onclose(function () {
            onConnectionClose();
        });

        hubConnection.start().then(function () {
            $rootScope.$apply(() => sCallback());
        }, (e) => { 
            $rootScope.$apply(function () {
                if (eCallback) {
                    eCallback(e);
                }
            });
        });
    }

    function createNewHubConnection() {
        return new signalR.HubConnectionBuilder()
            .withUrl(API_SETTINGS.uri + uri)
            .build();
    }

    function on(eventName, sCallback) {
        createHubConnectionIfNotExists();
        hubConnection.on(eventName, data => {
            $rootScope.$apply(() => {
                if (sCallback) {
                    sCallback(data);
                }
            });
        });
    }

    function invoke(...args) {
        createHubConnectionIfNotExists();
        if (!hubConnection || hubConnection.connectionState === 'Disconnected') {
            return connect(() => {
                return hubConnection.invoke(...args);
            });
        } else {
            return hubConnection.invoke(...args);
        }
    }

    function createHubConnectionIfNotExists() {
        if (!hubConnection) {
            hubConnection = createNewHubConnection();
        }
    }

    function close() {
        if (hubConnection) {
            hubConnection.connection.stop();
            hubConnection = null;
        }
    }

    function withUri(uri) {
        this.uri = uri;
    }

    return {
        withUri: withUri,
        onConnectionClose: onConnectionClose,
        on: on,
        invoke: invoke,
        close: close
    };
}]);