/* Directives */
'use strict';

angular.module('maximuslife.directives', [])
.directive('questMap', [function () {

    return {
        restrict: 'E',
        replace: true,
        transclude: false,
        template: '<div name="map" id="map" style="padding-top: 100px"></div>',
        link: function (scope, element, attrs) {

            attrs.$observe("mapData", function (value) {
                if (value) {
                    drawMap();
                }
            });

            function drawMap() {

                element.empty();

                if (scope.data && scope.data.checkpoints && scope.data.checkpoints.length > 0) {

                    var count = scope.data.checkpoints.length;
                    var containerWidth = document.getElementById('map').offsetWidth;
                    var triangleRatio = Math.round(((containerWidth / count) / 2));
                    var borderLeftRight = triangleRatio;
                    var borderWidth = triangleRatio;
                    var marginleft = count > 5 ? 0 : 20;
                    var marginTop = triangleRatio;
                    var bottomWidth = 0;

                    var circleHtml = "";
                    var templates = [];

                    if (null != scope.data) {

                        for (var index = 0; index < count; index++) {

                            var checkpoint = scope.data.checkpoints[index];

                            var borderLR = borderLeftRight
                            var bottomWidth = borderWidth;

                            var mLeft = marginleft + 2;
                            var mTop = marginTop;

                            var domClass = ((checkpoint.estimatedDate && checkpoint.completionDate) || checkpoint.isCurrent)
                                         ? 'revhr-map-green' : 'revhr-map-gray';

                            var triangleStyle = "border-bottom-style: solid; border-bottom-width: " + bottomWidth
                                              + "px; border-left : " + borderLR + "px solid transparent;  border-right : "
                                              + borderLR + "px solid transparent; margin-top: -" + mTop + "px; margin-left: "
                                              + mLeft + "px; height: 0px; width: 0px;";

                            var triangleHtml = '<div class="' + domClass + '" style="' + triangleStyle + '"></div> ';

                            if (checkpoint.isCurrent) {

                                var circleStyle = "margin-top: -" + (mTop + (bottomWidth / 2))
                                                + "px; margin-left: " + (mLeft + (borderLR - 10)) + "px;";
                                circleHtml = '<div class="revhr-map-circle" style="' + circleStyle
                                            + '">' + scope.data.checkinsActual + '</div> ';
                            }

                            templates.push(triangleHtml);

                            borderLeftRight = borderLR + 10;
                            borderWidth = bottomWidth + 16;
                            marginleft = mLeft + 24;
                            marginTop = mTop + 16;
                        }

                        for (var index = templates.length; index >= 0 ; index--) {
                            element.append(templates[index - 1]);
                        }

                        element.append(circleHtml);
                    }
                }
            }
        }
    }

}]);