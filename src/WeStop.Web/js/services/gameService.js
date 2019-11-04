angular.module('WeStop').factory('$game', ['$hub', function ($hub) {

    $hub.withUri('/game');

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