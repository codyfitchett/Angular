/* globals angular */
angular.module('maximuslife', ['ionic', 'maximuslife.services', 'maximuslife.controllers', 'maximuslife.directives'])
.run(function($ionicPlatform) {
  $ionicPlatform.ready(function() {
	if(window.StatusBar) {
	  // org.apache.cordova.statusbar required
	  StatusBar.styleDefault();
	}
  });
})
.config(['$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {
  'use strict';
    $stateProvider
    .state('login', {
        url: '/landing',
        views: {
            '@': {
                templateUrl: 'templates/login/landing.html',
                controller: 'LoginController'
            }
        }
    })
    .state('login.regprofile', {
        url: '/profile',
        views: {
            '@': {
                templateUrl: 'templates/login/profile.html',
                controller: 'LoginController'
            }
        }
    })
    .state('login.regcredential', {
        url: '/credential',
        views: {
            '@': {
                templateUrl: 'templates/login/credential.html',
                controller: 'LoginController'
            }
        }
    })
    .state('login.success', {
        url: '/success',
        views: {
            '@': {
                templateUrl: 'templates/login/regsuccess.html',
                controller: 'LoginController'
            }
        }
    })
    .state('login.start', {
        url: '/start',
        views: {
            '@': {
                templateUrl: 'templates/login/start.html',
                controller: 'LoginController'
            }
        }
    })
    .state('login.login', {
        url: '/login',
        views: {
            '@': {
                templateUrl: 'templates/login/login.html',
                controller: 'LoginController'
            }
        }
    })
	.state('categories', {
	    url: '/categories',
		views: {
			'@' : {
				templateUrl: 'templates/categories.html',
				controller: 'CategoryController'
			}
		}
	})
	.state('child-categories', {
	    url: '/categories/:categoryId',
		views: {
			'@' : {
				templateUrl: 'templates/child-categories.html',
				controller: 'ChildCategoryController'
			}
		}
	})
	.state('quests-detail', {
	    url: '/quests/:questId',
		views: {
			'@' : {
				templateUrl: 'templates/quests-detail.html',
				controller: 'QuestController'
			}
		}
	})
	.state('join-quest', {
	    url: '/quests/:questId/join',
		views: {
			'@' : {
				templateUrl: 'templates/join-quest.html',
				controller: 'QuestController'
			}
		}
	})
    .state('quest-declaration', {
        url: '/quests/:questId/join/declaration',
        views: {
            '@': {
                templateUrl: 'templates/quest-declaration.html',
                controller: 'QuestController'
            }
        }
    })
    .state('quest-map', {
        url: '/quests/:questId/map',
		views: {
			'@' : {
				templateUrl: 'templates/quest-map.html',
				controller: 'QuestController'
			}
		}
    })
	.state('quest-map.chart-map', {
        url: '/chart-map',
		views: {
			'chartView' : {
				templateUrl: 'templates/mapcharts/map.html'
			}
		}
    })
	.state('quest-map.chart-pulse', {
        url: '/chart-pulse',
		views: {
			'chartView' : {
				templateUrl: 'templates/mapcharts/pulse.html'
			}
		}
    })
	.state('quest-map.chart-timeline', {
        url: '/chart-timeline',
		views: {
			'chartView' : {
				templateUrl: 'templates/mapcharts/timeline.html'
			}
		}
    })
	.state('checkin', {
        url: '/quests/:questId/map/checkin',
		views: {
			'@' : {
				templateUrl: 'templates/checkin.html',
				controller: 'QuestController'
			}
		}
	})
	.state('builder', {
		abstract: true,
		url: '/quests/:questId/builder',
		templateUrl: 'templates/builder.html',
		controller: 'QuestController'
	})
	.state('builder.timeline', {
		url: '/timeline',
		templateUrl: 'templates/builder-timeline.html'
	})
	.state('builder.checkpoints', {
		url: '/checkpoints',
		templateUrl: 'templates/builder-checkpoints.html'
	})
    ;

	$urlRouterProvider.otherwise('/landing');
}])
;
