(function () {
	'use strict';

	var app = angular.module('myapp', [
		'ui.router',
		'jcs.angular-http-batch'
	]);

	app.config(["$stateProvider", "$urlRouterProvider",
		function ($stateProvider, $urlRouterProvider) {
			$urlRouterProvider.otherwise("/home");
			$stateProvider.state("home", {
				url: "/home", templateUrl: "/templates/home.html",
				resolve: {
					TestClassOneService: "TestClassOneService",

					testClassOne: ["TestClassOneService", function (TestClassOneService) {
						return TestClassOneService.getAll();
					}]
				}
			});
		}
	]);

	//app.config(['httpBatchConfigProvider', function (httpBatchConfigProvider) {
	//	httpBatchConfigProvider.setAllowedBatchEndpoint(
	//		// root endpoint url
	//		'http://localhost:52857',

	//		// endpoint batch address
	//		'http://localhost:52857/api/$batch',

	//		// optional configuration parameters
	//		{
	//			maxBatchedRequestPerCall: 20
	//		});
	//	}
	//]);

})();