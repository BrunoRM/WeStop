angular.module('WeStop').factory('$lobby', ['$hub', function ($hub) {

    $hub.withUri('/lobby');

    function leave () {
        $hub.close();
    }

    return {
        onConnectionClose: $hub.onConnectionClose,
        on: $hub.on,
        invoke: $hub.invoke,
        leave: leave
    };

}]);