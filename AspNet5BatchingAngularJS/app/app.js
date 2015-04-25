(function () {
	'use strict';

	var app = angular.module('myapp', [
		'ui.router',
		'jcs.angular-http-batch'
	]);

	app.config(["$stateProvider", "$urlRouterProvider",
		function ($stateProvider, $urlRouterProvider) {
			$urlRouterProvider.otherwise("/index");
			$stateProvider.state("index", {
				url: "/index", templateUrl: "/templates/home.html",
				resolve: {
					testClassOneService: "testClassOneService",

					testClassOne: ["testClassOneService", function (testClassOneService) {
						return testClassOneService.getAll();
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