(function () {
	'use strict';

	var app = angular.module('app', [
		'jcs.angular-http-batch'

	]);

	app.config(['httpBatchConfigProvider', function (httpBatchConfigProvider) {
		httpBatchConfigProvider.setAllowedBatchEndpoint(
				// root endpoint url
				'http://localhost:52857',

				// endpoint batch address
				'http://localhost:52857/api/$batch',

				// optional configuration parameters
				{
					maxBatchedRequestPerCall: 20
				});
	}
	]);


	angular.module('myApp', ['jcs.angular-http-batch']);

})();