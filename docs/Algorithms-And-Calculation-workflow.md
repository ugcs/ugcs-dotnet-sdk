
# Description of algorithms and their parameters

Each algorithm has its own set of parameters (part common, part specific). 
A detailed description of the parameters and a description of the calculation features are given in the following topics:

- <a href="#areascan">Area scan</a>
- <a href="#circle">Circle</a>
- <a href="#corridor">Corridor mapping tool</a>
- <a href="#creeping">Creeping Line Search Pattern</a>
- <a href="#square">Expanding Square Search Pattern</a>
- <a href="#facade">Facade scanner</a>
- <a href="#landing">Landing</a>
- <a href="#perimeter">Perimeter</a>
- <a href="#photogrammetrytool">Photogrammetry tool</a>
- <a href="#takeoff">Takeoff</a>
- <a href="#waypoint">Waypoint</a>

<a name="areascan"></a><h2>Area scan</h2>
<table>
<thead>
<tr>
<th tabindex="0" scope="col" data-column="0">
<div>Name</div>
</th>
<th tabindex="0" colspan="1" scope="col" data-column="1">
<div>Name in UCS</div>
</th>
<th tabindex="0" colspan="1" scope="col" data-column="2">
<div>Type</div>
</th>
<th tabindex="0" scope="col" data-column="3">
<div>Purpose</div>
</th>
<th tabindex="0" colspan="1" scope="col" data-column="4">
<div>Default value</div>
</th>
<th tabindex="0" colspan="1" scope="col" data-column="5">
<div>Is the last value storing?</div>
</th>
</tr>
</thead>
<tbody>
<tr>
<td>Flight speed</td>
<td colspan="1"> </td>
<td colspan="1">Double</td>
<td>Flight speed</td>
<td colspan="1">
<p>The value corresponds to the value specified the previous segment of the route.</p>
<p>If there are no segments, the value is taken from the profile = default ground speed.</p>
</td>
<td colspan="1"> </td>
</tr>
<tr>
<td>Turn type</td>
<td colspan="1"> </td>
<td colspan="1">Selection</td>
<td>
<p>Turn type. Can be one of the following values:</p>
<ul>
<li>STOP_AND_TURN;</li>
<li>STRAIGHT;</li>
<li>SPLINE;</li>
<li>BANK_TURN.</li>
</ul>
<p>When changing the machine, if in the new list of allowed turn types, then it remains</p>
</td>
<td colspan="1">
<p>Turn type is selected from the list of available turn types for a profile.</p>
<p>The default value is the default value for the profile</p>
</td>
<td colspan="1">(from 2.12 per profile)</td>
</tr>
<tr>
<td colspan="1">Flight height</td>
<td colspan="1"> </td>
<td colspan="1">Double</td>
<td colspan="1">Flight height</td>
<td colspan="1">
<ol>
<li>In case is the first scheduling algorithm in the route, the altitude value is formed according to the rule:<br />
<ol>
<li>type of the route is AGL, altitude type of the area scan is AMSL = value the route settings from the emergency return altitude field is added to the elevation at the first point of the area scan set on the map. Accordingly, this value is emergency return altitude + elevation and will be Flight height</li>
<li>type of the route is AGL, altitude type of the area scan is AGL = value is taken from the route settings from the emergency return altitude field</li>
<li>type of the route is AMSL, altitude type of the area scan is AMSL = value is taken from the route settings from the emergency return altitude field</li>
<li>type of the route is AMSL, altitude type of the area scan is AGL = value from the route settings from the emergency return altitude field is subtracted from the elevation at the first point of the area scan set on the map. Accordingly, this value is emergency return altitude - elevation and will be Flight height</li>
</ol>
</li>
<li>In case the area is followed by another route algorithm:
<ol>
<li>type of the route is AGL, altitude type of the area scan is AMSL = altitude of the last scheduling algorithm is added to the elevation at the first point of the area scan set on the map. Accordingly, this value of altitude + elevation and is the value of Flight height</li>
<li>type of the route is AGL, altitude type of the area scan is AGL = value is taken equal to the altitude of the last scheduling algorithm</li>
<li><span> type of the route is AMSL, altitude type of the area scan is AMSL = the value is taken equal to the altitude of the last scheduling algorithm<br /></span></li>
<li>type of the route is AMSL, altitude type of the area scan is of the last scheduling algorithm is subtracted from the elevation at the first point of the area scan set on the map. Accordingly, this value of altitude - elevation and is the value of Flight height</li>
</ol>
</li>
</ol>
<p>3. When altitude type is changed in area scan, flight height is reset</p>
</td>
<td colspan="1"> </td>
</tr>
<tr>
<td colspan="1">Altitude type</td>
<td colspan="1"> </td>
<td colspan="1">Selection</td>
<td colspan="1">
<p>Usage altitude type:</p>
<ul>
<li>WGS84 - route traversing one given AMSL-altitude;</li>
<li>AGL - route traversing one given AGL-altitude.</li>
</ul>
</td>
<td colspan="1">AMSL</td>
<td colspan="1"> </td>
</tr>
<tr>
<td colspan="1">Side distance</td>
<td colspan="1"> </td>
<td colspan="1">Double</td>
<td colspan="1">Side distance.</td>
<td colspan="1"> </td>
<td colspan="1"> </td>
</tr>
<tr>
<td colspan="1">Direction angle (0-360)</td>
<td colspan="1"> </td>
<td colspan="1">Double</td>
<td colspan="1">
<p>Direction angle (0-360)</p>
<p>Angle of rotation of the grid.</p>
</td>
<td colspan="1">0</td>
<td colspan="1"> </td>
</tr>
<tr>
<td colspan="1">Avoid obstacles</td>
<td colspan="1"> </td>
<td colspan="1">Boolean</td>
<td colspan="1">Avoid obstacles</td>
<td colspan="1"> </td>
<td colspan="1"> </td>
</tr>
<tr>
<td colspan="1">Action execution</td>
<td colspan="1"> </td>
<td colspan="1">Selection</td>
<td colspan="1">
<p>Action execution. Shows when actions are to be performed:</p>
<ul>
<li>ONLY_AT_START - only at the starting point;</li>
<li>ACTIONS_EVERY_POINT - at each point;</li>
<li>ACTIONS_ON_FORWARD_PASSES - at each point of passes. But when turning, the camera should turn off.</li>
</ul>
</td>
<td colspan="1">point</td>
<td colspan="1"> </td>
</tr>
<tr>
<td colspan="1">Overshoot</td>
<td colspan="1"> </td>
<td colspan="1">Double</td>
<td colspan="1">
<p>Overshoot</p>
<p>beyond the snake, which must be passed to properly and completely pass through the snake taking into account the features of the rotation of the plane.</p>
</td>
<td colspan="1"> </td>
<td colspan="1"> </td>
</tr>
<tr>
<td colspan="1">Overshoot speed</td>
<td colspan="1"> </td>
<td colspan="1">Double</td>
<td colspan="1">Speed for overshoot part</td>
<td colspan="1"> </td>
<td colspan="1"> </td>
</tr>
<tr>
<td colspan="1">Allow partial calculation</td>
<td colspan="1"> </td>
<td colspan="1">Boolean</td>
<td colspan="1">Whether to allow partial path computation or to throw an error when some of the points are inaccessible for some reason</td>
<td colspan="1"> </td>
<td colspan="1"> </td>
</tr>
<tr>
<td colspan="1">AGL Tolerance</td>
<td colspan="1"> </td>
<td colspan="1">Double</td>
<td colspan="1">
<p>Allowable height difference, when you can not put additional points. An additional point is placed if it goes beyond this boundary.</p>
<p>If not set should be treated as 0 (zero)</p>
</td>
<td colspan="1">3</td>
<td colspan="1"> </td>
</tr>
<tr>
<td colspan="1">No actions at last point</td>
<td colspan="1"> </td>
<td colspan="1">Boolean</td>
<td colspan="1">Do not perform actions at the last point</td>
<td colspan="1"> </td>
<td colspan="1"> </td>
</tr>
</tbody>
</table>

<a name="circle"></a><h2>Circle</h2>
<table>
<thead>
<tr>
<th tabindex="0" scope="col" data-column="0">
<div>Name</div>
</th>
<th tabindex="0" colspan="1" scope="col" data-column="1">
<div>Name in UCS</div>
</th>
<th tabindex="0" colspan="1" scope="col" data-column="2">
<div>Type</div>
</th>
<th tabindex="0" scope="col" data-column="3">
<div>Purpose</div>
</th>
<th tabindex="0" colspan="1" scope="col" data-column="4">
<div>Default value</div>
</th>
</tr>
</thead>
<tbody>
<tr>
<td>Flight speed</td>
<td colspan="1">speed</td>
<td colspan="1">Double</td>
<td>Flight speed</td>
<td colspan="1">
<p>The value corresponds to the value specified for the previous segment of the route.</p>
<p>If there are no segments, the value is taken from the profile = default ground speed.</p>
</td>
</tr>
<tr>
<td>Turn type</td>
<td colspan="1">wpTurnType</td>
<td colspan="1">Selection</td>
<td>
<p>Turn type. Can be one of the following values:</p>
<ul>
<li>STOP_AND_TURN;</li>
<li>STRAIGHT;</li>
<li>SPLINE;</li>
<li>BANK_TURN.</li>
</ul>
<p>When changing the machine, if selected value in the new list of allowed turn types, then it remains</p>
</td>
<td colspan="1">
<p>Turn type is selected from the list of available turn types for a profile.</p>
<p>The default value is the default value for the profile</p>
<p> </p>
</td>
</tr>
<tr>
<td>Number of laps</td>
<td colspan="1">loops</td>
<td colspan="1">Integer</td>
<td>Number of repeated flights</td>
<td>1</td>
</tr>
<tr>
<td colspan="1">Fly cloclwise</td>
<td colspan="1">flightClockwise</td>
<td colspan="1">Boolean</td>
<td colspan="1">Fly cloclwise</td>
<td colspan="1">yes</td>
</tr>
<tr>
<td colspan="1">Number of approximating points</td>
<td colspan="1">basePointsQnt</td>
<td colspan="1">Integer</td>
<td colspan="1">The number of approximating points. Allows you to specify a fixed value of the points that will be used to form a circle path.</td>
<td colspan="1">-</td>
</tr>
<tr>
<td colspan="1">Follow terrain</td>
<td colspan="1">followTerrain</td>
<td colspan="1">Boolean</td>
<td colspan="1">Follow terrain</td>
<td colspan="1">yes</td>
</tr>
<tr>
<td colspan="1">Action execution</td>
<td colspan="1">actionExecution</td>
<td colspan="1">Selection</td>
<td colspan="1">
<p>Action execution. Shows when actions are to be performed:</p>
<ul>
<li>ONLY_AT_START - only at the starting point;</li>
<li>ACTIONS_EVERY_POINT - at each point.</li>
</ul>
</td>
<td colspan="1">Every point</td>
</tr>
<tr>
<td colspan="1">Avoid obstacles</td>
<td colspan="1">avoidObstacles</td>
<td colspan="1">Boolean</td>
<td colspan="1">Avoid obstacles</td>
<td colspan="1">yes</td>
</tr>
<tr>
<td colspan="1">Avoid terrain</td>
<td colspan="1">avoidTerrain</td>
<td colspan="1">Boolean</td>
<td colspan="1">Avoid terrain</td>
<td colspan="1">yes</td>
</tr>
<tr>
<td colspan="1">No actions at last point</td>
<td colspan="1">noActionsAtLastPoint</td>
<td colspan="1">Boolean</td>
<td colspan="1">Do not perform actions at the last point</td>
<td colspan="1">yes</td>
</tr>
</tbody>
</table>

<a name="corridor"></a><h2>Corridor mapping tool</h2>
<table data-number-column="false">
<tbody>
<tr>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="552"><strong data-renderer-mark="true">Parameter</strong></p>
</td>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="565"><strong data-renderer-mark="true">Definition</strong></p>
</td>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="579"><strong data-renderer-mark="true">Mandatory</strong></p>
</td>
</tr>
<tr>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="594">Turn type</p>
</td>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="607">Turn type. Can be one of the following values:</p>
<ul>
<li>
<p data-renderer-start-pos="657">STOP_AND_TURN;</p>
</li>
<li>
<p data-renderer-start-pos="675">STRAIGHT;</p>
</li>
<li>
<p data-renderer-start-pos="688">SPLINE;</p>
</li>
<li>
<p data-renderer-start-pos="699">BANK_TURN.</p>
</li>
</ul>
<p data-renderer-start-pos="713">When changing the machine, if selected value in the new list of allowed turn types, then it remains</p>
</td>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="816">Yes</p>
</td>
</tr>
<tr>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="825">Width</p>
</td>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="834">Corridor width in meters. Means that we add Width/2 margin to the left and right from the center line.</p>
<p data-renderer-start-pos="938">Integer, entered in inspector. By default 30 or previously entered value.</p>
</td>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="1015">Yes</p>
</td>
</tr>
<tr>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="1024">Speed</p>
</td>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="1033">Speed along the path. Default is value from vehicle profile or previously entered value</p>
</td>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="1124">Yes</p>
</td>
</tr>
<tr>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="1133">Camera</p>
</td>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="1143">Drop down list with cameras attached to the vehicle profile. Default is 1<sup data-renderer-mark="true">st</sup> camera in the profile or previously selected value.</p>
</td>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="1274">Yes</p>
</td>
</tr>
<tr>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="1283">GSD</p>
</td>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="1290">Ground sample distance. Default is 2cm or previously entered value.</p>
</td>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="1361">Yes</p>
</td>
</tr>
<tr>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="1370">Forward overlap</p>
</td>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="1389">Percentage of forward overlap. Default is 60% or previously entered value.</p>
</td>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="1467">Yes</p>
</td>
</tr>
<tr>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="1476">Side overlap</p>
</td>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="1492">Percentage of side overlap. Default is 30% or previously entered value.</p>
</td>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="1567">Yes</p>
</td>
</tr>
<tr>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="1576">Camera top facing forward</p>
</td>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="1605">Camera top facing forward</p>
</td>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="1634">Yes</p>
</td>
</tr>
<tr>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="1643">AGL Tolerance</p>
</td>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="1660">Allowable height difference, when you can not put additional points. An additional point is placed if it goes beyond this boundary.</p>
<p data-renderer-start-pos="1793">If not set should be treated as 0 (zero)</p>
</td>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="1837">No</p>
</td>
</tr>
<tr>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="1845">Avoid obstacles</p>
</td>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="1864">Avoid obstacles</p>
</td>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="1883">Yes</p>
</td>
</tr>
<tr>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="1892">Altitude type</p>
</td>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="1909">AMSL/AGL. Default is AGL, or previously entered value.</p>
</td>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="1967">Yes</p>
</td>
</tr>
<tr>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="1976">Action execution</p>
</td>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="1996">Action execution. Shows when actions are to be performed:</p>
<ul>
<li>
<p data-renderer-start-pos="2057">ONLY_AT_START - only at the starting point;</p>
</li>
<li>
<p data-renderer-start-pos="2104">ACTIONS_EVERY_POINT - at each point;</p>
</li>
<li>
<p data-renderer-start-pos="2144">ACTIONS_ON_FORWARD_PASSES - at each point of passes. But when turning, the camera should turn off.</p>
</li>
</ul>
</td>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="2248">Yes</p>
</td>
</tr>
<tr>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="2257">No actions at last point</p>
</td>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="2285">Do not perform actions at the last point</p>
</td>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="2329">Yes</p>
</td>
</tr>
<tr>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="2338">Corridor center line</p>
</td>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="2362">Polyline. Defined manually on map or can be imported from KML or CSV. Waypoints have no altitude and tied to the elevation level.</p>
</td>
<td colspan="1" rowspan="1">
<p data-renderer-start-pos="2495">Yes</p>
</td>
</tr>
</tbody>
</table>

<a name="creeping"></a><h2>Corridor mapping tool</h2>
<table>
<thead>
<tr>
<th tabindex="0" scope="col" data-column="0">
<div>Name</div>
</th>
<th tabindex="0" colspan="1" scope="col" data-column="1">
<div>Name in UCS</div>
</th>
<th tabindex="0" colspan="1" scope="col" data-column="2">
<div>Type</div>
</th>
<th tabindex="0" scope="col" data-column="3">
<div>Purpose</div>
</th>
<th tabindex="0" colspan="1" scope="col" data-column="4">
<div><span>Default value</span></div>
</th>
<th tabindex="0" colspan="1" scope="col" data-column="5">
<div><span><span>Is the last value storing?</span><br /></span></div>
</th>
</tr>
</thead>
<tbody>
<tr>
<td>Flight speed</td>
<td colspan="1">speed</td>
<td colspan="1">Double</td>
<td>Flight speed</td>
<td colspan="1">
<p>The value corresponds to the value specified for the previous segment of the route.</p>
<p>If there are no segments, the value is taken from the profile = default ground speed.</p>
</td>
<td colspan="1">no</td>
</tr>
<tr>
<td>Turn type</td>
<td colspan="1">wpTurnType</td>
<td colspan="1">Selection</td>
<td>
<p>Turn type. Can be one of the following values:</p>
<ul>
<li>STOP_AND_TURN;</li>
<li>STRAIGHT;</li>
<li>SPLINE;</li>
<li>BANK_TURN.</li>
</ul>
<p>When changing the machine, if selected value in the new list of allowed turn types, then it remains</p>
</td>
<td colspan="1">
<p>Turn type is selected from the list of available turn types for a profile.</p>
<p>The default value is the default value for the profile</p>
</td>
<td colspan="1"><span>yes (from 2.12 per profile</span><span>)</span></td>
</tr>
<tr>
<td>Camera</td>
<td colspan="1">camera</td>
<td colspan="1">Selection</td>
<td>
<p><span>Camera.</span></p>
<p>Search spacing is derived from the camera profile parameters.</p>
<p><span>Given</span></p>
<div><span>d - sensor width</span></div>
<div>
<div><span>f - focal length</span></div>
</div>
<div><span>fov = 2 * arctan(d / (2 * f))</span></div>
<p><span>and thus:</span></p>
<div>
<div><span> </span></div>
<div><span>tan(fov / 2) = d / (2 * f) (eq. 1)</span></div>
</div>
<div>
<div><span>spacing</span> <span>= </span><span>(2 * height) * </span><span>tan(fov / 2)</span></div>
</div>
<p><span>using (eq. 1):</span></p>
<div><span> </span></div>
<div>
<div><span>spacing</span><span>= </span><span>(2 * height) * </span><span>(</span><span>d / (2 * f)</span><span>)</span></div>
<div><span> </span></div>
So the final equation for spacing is:
<div>
<div><span> </span></div>
<div><span>spacing</span><span>= </span><span>(height * d) / f</span></div>
<div><span> </span></div>
where height - AGL altitude (parameter), d - sensor width (camera profile), f - sensor focal length (camera profile).</div>
</div>
<div></div>
</td>
<td colspan="1">Select the camera available for this profile.</td>
<td colspan="1">yes</td>
</tr>
<tr>
<td colspan="1"><span>Flight height</span></td>
<td colspan="1">height</td>
<td colspan="1">Double</td>
<td colspan="1">
<div>
<p><span>Flight height <span>AGL</span></span></p>
</div>
</td>
<td colspan="1">
<p>The value corresponds to the value specified for the previous segment of the route.</p>
<p>If there are no segments, the value is taken from the profile = value the route settings from the emergency return altitude field.</p>
</td>
<td colspan="1">yes</td>
</tr>
<tr>
<td colspan="1"><span>AGL Tolerance</span></td>
<td colspan="1">tolerance</td>
<td colspan="1"><span>Double</span></td>
<td colspan="1">
<p>Allowable height difference, when you can not put additional points. An additional point is placed if it goes beyond this boundary.</p>
<p>If not set should be treated as 0 (zero)</p>
</td>
<td colspan="1">3</td>
<td colspan="1">yes</td>
</tr>
<tr>
<td colspan="1">Direction angle (0-360)</td>
<td colspan="1">directionAngle</td>
<td colspan="1">Double</td>
<td colspan="1">
<p>Direction angle (0-360)</p>
<p>Angle of rotation of the grid.</p>
</td>
<td colspan="1">0</td>
<td colspan="1">yes</td>
</tr>
<tr>
<td colspan="1"><span>Side overlap, %</span></td>
<td colspan="1"> <span>overlapSide</span></td>
<td colspan="1">Double</td>
<td colspan="1">
<p><span>Side overlap</span>, %<span> (lateral overlap)</span></p>
<p><span><span><span>Affects spacing in a following way: </span></span></span></p>
<p><span><span><span>spacing</span> = (<span>1</span> - (<span>overlap / 100)</span>) * </span>spacing</span></p>
<p> </p>
<p><span>Max. side.overlap = 50% (causes 100% overlapping)</span></p>
</td>
<td colspan="1"> 10</td>
<td colspan="1">yes</td>
</tr>
<tr>
<td colspan="1">Avoid obstacles</td>
<td colspan="1">avoidObstacles</td>
<td colspan="1"><span>Boolean</span></td>
<td colspan="1"><span>Avoid obstacles</span></td>
<td colspan="1">yes</td>
<td colspan="1"><span>yes</span></td>
</tr>
<tr>
<td colspan="1">Action execution</td>
<td colspan="1">actionExecution</td>
<td colspan="1"><span>Selection</span></td>
<td colspan="1">
<p>Action execution. Shows when actions are to be performed:</p>
<ul>
<li>ONLY_AT_START - only at the starting point;</li>
<li>ACTIONS_EVERY_POINT - at each point;</li>
<li><span>(PLANNED FOR 3.2) </span>ACTIONS_ON_FORWARD_PASSES - at each point of passes. But when turning, the camera should turn off. </li>
</ul>
</td>
<td colspan="1">every point</td>
<td colspan="1"><span>yes</span></td>
</tr>
<tr>
<td colspan="1">No actions at last point</td>
<td colspan="1">noActionsAtLastPoint</td>
<td colspan="1">Boolean</td>
<td colspan="1">Do not perform actions at the last point</td>
<td colspan="1"><span>yes</span></td>
<td colspan="1"> </td>
</tr>
</tbody>
</table>

<a name="creeping"></a><h2>Expanding Square Search Pattern</h2>
<table>
<thead>
<tr>
<th tabindex="0" scope="col" data-column="0">
<div>Name</div>
</th>
<th tabindex="0" colspan="1" scope="col" data-column="1">
<div>Name in UCS</div>
</th>
<th tabindex="0" colspan="1" scope="col" data-column="2">
<div>Type</div>
</th>
<th tabindex="0" scope="col" data-column="3">
<div>Purpose</div>
</th>
<th tabindex="0" colspan="1" scope="col" data-column="4">
<div><span>Default value</span></div>
</th>
<th tabindex="0" colspan="1" scope="col" data-column="5">
<div><span><span>Is the last value storing?</span><br /></span></div>
</th>
</tr>
</thead>
<tbody>
<tr>
<td colspan="1">Altitude AGL</td>
<td colspan="1">height</td>
<td colspan="1">Double</td>
<td colspan="1"><span>Flight height AGL</span></td>
<td colspan="1">
<p>The value corresponds to the value specified for the previous segment of the route.</p>
<p>If there are no segments, the value is taken from the profile = value the route settings from the emergency return altitude field.</p>
</td>
<td colspan="1">yes</td>
</tr>
<tr>
<td>Flight speed</td>
<td colspan="1">speed</td>
<td colspan="1">Double</td>
<td>Flight speed</td>
<td colspan="1">
<p>The value corresponds to the value specified for the previous segment of the route.</p>
<p>If there are no segments, the value is taken from the profile = default ground speed.</p>
</td>
<td colspan="1">no</td>
</tr>
<tr>
<td>Turn type</td>
<td colspan="1">wpTurnType</td>
<td colspan="1">Selection</td>
<td>
<p>Turn type. Can be one of the following values:</p>
<ul>
<li>STOP_AND_TURN;</li>
<li>STRAIGHT;</li>
<li>SPLINE;</li>
<li>BANK_TURN.</li>
</ul>
<p>When changing the machine, if selected value in the new list of allowed turn types, then it remains</p>
</td>
<td colspan="1">
<p>Turn type is selected from the list of available turn types for a profile.</p>
<p>The default value is the default value for the profile</p>
</td>
<td colspan="1"><span>yes (from 2.12 per profile</span><span>)</span></td>
</tr>
<tr>
<td>Camera</td>
<td colspan="1">camera</td>
<td colspan="1">Selection</td>
<td>
<p><span>Camera.</span></p>
<p>Search spacing is derived from the camera profile parameters.</p>
</td>
<td colspan="1">Select the camera available for this profile.</td>
<td colspan="1">yes</td>
</tr>
<tr>
<td colspan="1"><span>AGL Tolerance</span></td>
<td colspan="1">tolerance</td>
<td colspan="1"><span>Double</span></td>
<td colspan="1">
<p>Allowable height difference, when you can not put additional points. An additional point is placed if it goes beyond this boundary.</p>
<p>If not set should be treated as 0 (zero)</p>
</td>
<td colspan="1">3</td>
<td colspan="1">yes</td>
</tr>
<tr>
<td colspan="1">Search radius</td>
<td colspan="1">searchRadius</td>
<td colspan="1"><span>Double</span></td>
<td colspan="1">
<p><span>Radius to cover.</span></p>
<p><span>Distance from a search center to the outer segment that must be covered by the payload footprint</span></p>
</td>
<td colspan="1">350</td>
<td colspan="1">yes</td>
</tr>
<tr>
<td colspan="1">Direction angle (0-360)</td>
<td colspan="1">directionAngle</td>
<td colspan="1">Double</td>
<td colspan="1">
<p>Direction angle (0-360)</p>
<p>Angle of rotation of the grid.</p>
</td>
<td colspan="1">0</td>
<td colspan="1">yes</td>
</tr>
<tr>
<td colspan="1"><span>Side overlap, %</span></td>
<td colspan="1"> <span>overlapSide</span></td>
<td colspan="1">Double</td>
<td colspan="1">
<p><span>Side overlap</span>, %<span> (lateral overlap)</span></p>
<p><span><span><span>Affects spacing in a following way: </span></span></span></p>
<p><span><span><span>spacing</span> = (<span>1</span> - (<span>overlap / 100)</span>) * </span>spacing</span></p>
</td>
<td colspan="1"> 10</td>
<td colspan="1">yes</td>
</tr>
<tr>
<td colspan="1">Avoid obstacles</td>
<td colspan="1">avoidObstacles</td>
<td colspan="1"><span>Boolean</span></td>
<td colspan="1"><span>Avoid obstacles</span></td>
<td colspan="1">yes</td>
<td colspan="1"><span>yes</span></td>
</tr>
<tr>
<td colspan="1">Action execution</td>
<td colspan="1">actionExecution</td>
<td colspan="1"><span>Selection</span></td>
<td colspan="1">
<p>Action execution. Shows when actions are to be performed:</p>
<ul>
<li>ONLY_AT_START - only at the starting point;</li>
<li>ACTIONS_EVERY_POINT - at each point;</li>
</ul>
</td>
<td colspan="1">every point</td>
<td colspan="1"><span>yes</span></td>
</tr>
<tr>
<td colspan="1">No actions at last point</td>
<td colspan="1">noActionsAtLastPoint</td>
<td colspan="1">Boolean</td>
<td colspan="1">Do not perform actions at the last point</td>
<td colspan="1"><span>yes</span></td>
<td colspan="1"> </td>
</tr>
</tbody>
</table>

<a name="facade"></a><h2>Facade scanner</h2>
<table>
<tbody>
<tr>
<td>
<p><strong>Parameter</strong></p>
</td>
<td>
<p><strong>Control</strong></p>
</td>
<td>
<p><strong>Definition</strong></p>
</td>
</tr>
<tr>
<td>
<p>Left corner (A)</p>
</td>
<td>
<p>Lat, lon input with ability to point on map <br /><br />A button allowing to set current drone position is welcomed but not mandatory for the first version.</p>
</td>
<td>
<p>Coordinates of left corner (A) of the vertical plane where drone should fly. Point is always on the elevation level. Parameter is mandatory.</p>
</td>
</tr>
<tr>
<td>
<p>Right corner(B)</p>
</td>
<td>
<p>Lat, lon input with ability to point on map <br /><br />A button allowing to set current drone position is welcomed but not mandatory for the first version.</p>
</td>
<td>
<p>Coordinates of right corner (B) of the vertical plane where drone should fly. Point is always on the elevation level. Parameter is mandatory.</p>
</td>
</tr>
<tr>
<td>
<p>Minimum height (AGL)</p>
</td>
<td>
<p>Input field allowing to specify float number <br /><br />Visual adjustment tool is optional for the first version but welcomed</p>
</td>
<td>
<p>Specifies minimum height above highest point on A-B line. <br /><br />AGL only irrespective to route settings. <br /><br />Parameter is mandatory. <br /><br />Should be in a range of allowed altitude for the vehicle profile: «Safe height over terrain» and «Max altitude, AGL» <br /><br />Also can not be greater than «Maximum height (AGL)».</p>
</td>
</tr>
<tr>
<td>
<p>Maximum height (AGL)</p>
</td>
<td>
<p>Input field allowing to specify float number <br /><br />Visual adjustment tool is optional for the first version but welcomed</p>
</td>
<td>
<p>Specifies maximum height above highest point on A-B line. <br /><br />AGL only irrespective to route settings. <br /><br />Parameter is mandatory <br /><br />Should be in a range of allowed altitude for the vehicle profile: «Safe height over terrain» and «Max altitude, AGL» <br /><br />Also can not be less than «Minimum height (AGL)»</p>
</td>
</tr>
<tr>
<td>
<p>Distance to facade</p>
</td>
<td>
<p>Input field allowing to specify float number</p>
</td>
<td>
<p>Specifies distance to the real vertical object from flight plane</p>
</td>
</tr>
<tr>
<td>
<p>Camera</p>
</td>
<td>
<p>Drop down list consisting of cameras attached to the vehicle profile</p>
</td>
<td>
<p>Parameter is mandatory</p>
</td>
</tr>
<tr>
<td>
<p>Forward overlap (%)</p>
</td>
<td>
<p>Input field allowing to specify integer number</p>
</td>
<td>
<p>Specifies overlap percentage between subsequent shots on a single forward pass <br /><br />Default 60% <br /><br />Parameter is mandatory <br /><br />Value should not exceed 90%</p>
</td>
</tr>
<tr>
<td>
<p>Side overlap (%)</p>
</td>
<td>
<p>Input field allowing to specify integer number</p>
</td>
<td>
<p>Specifies overlap percentage between shots on parallel forward passes <br /><br />Default 60% <br /><br />Parameter is mandatory <br /><br />Value should not exceed 90%</p>
</td>
</tr>
<tr>
<td>
<p>Pattern</p>
</td>
<td>
<p>Drop down with two values:</p>
<ul>
<li>Horizontal</li>
<li>Vertical</li>
</ul>
</td>
<td>
<p>Specifies scan pattern type. <br /><br />Default is vertical. <br /><br />Parameter is mandatory</p>
</td>
</tr>
<tr>
<td>
<p>Vertical speed</p>
</td>
<td>
<p>Input field allowing to specify float number</p>
</td>
<td>
<p>Specifies required vertical speed <br /><br />Default maximum vertical speed from profile <br /><br />Parameter is mandatory <br /><br />Value should not exceed maximum allowed vertical speed from profile</p>
</td>
</tr>
<tr>
<td>
<p>Horizontal speed</p>
</td>
<td>
<p>Input field allowing to specify float number</p>
</td>
<td>
<p>Specifies required horizontal speed <br /><br />Default maximum horizontal speed from profile divided by 2 <br /><br />Parameter is mandatory <br /><br />Value should not exceed maximum allowed horizontal speed from profile</p>
</td>
</tr>
<tr>
<td>
<p>Action execution</p>
</td>
<td>
<p>Drop down:</p>
<ul>
<li>Action in every point</li>
<li>Forward passes</li>
</ul>
</td>
<td>
<p>Parameter is mandatory</p>
</td>
</tr>
</tbody>
</table>

<a name="landing"></a><h2>Landing</h2>
<table><colgroup><col /><col /><col /><col /><col /></colgroup>
<thead>
<tr style="height: 20px;">
<th style="height: 20px;" tabindex="0" scope="col" data-column="0">
<div>Name</div>
</th>
<th style="height: 20px;" tabindex="0" colspan="1" scope="col" data-column="1">
<div>Name in UCS</div>
</th>
<th style="height: 20px;" tabindex="0" colspan="1" scope="col" data-column="2">
<div>Type</div>
</th>
<th style="height: 20px;" tabindex="0" scope="col" data-column="3">
<div>Purpose</div>
</th>
<th style="height: 20px;" tabindex="0" colspan="1" scope="col" data-column="4">
<div>Default value</div>
</th>
</tr>
</thead>
<tbody>
<tr style="height: 20px;">
<td style="height: 20px;" colspan="1">Avoid obstacles</td>
<td style="height: 20px;" colspan="1">avoidObstacles</td>
<td style="height: 20px;" colspan="1">Boolean</td>
<td style="height: 20px;" colspan="1">Avoid obstacles</td>
<td style="height: 20px;" colspan="1">yes</td>
</tr>
<tr style="height: 20px;">
<td style="height: 20px;" colspan="1">Avoid terrain</td>
<td style="height: 20px;" colspan="1">avoidTerrain</td>
<td style="height: 20px;" colspan="1">Boolean</td>
<td style="height: 20px;" colspan="1">Avoid terrain</td>
<td style="height: 20px;" colspan="1">yes</td>
</tr>
</tbody>
</table>

<a name="perimeter"></a><h2>Perimeter</h2>
<table>
<thead>
<tr>
<th tabindex="0" scope="col" data-column="0">
<div>Name</div>
</th>
<th tabindex="0" colspan="1" scope="col" data-column="1">
<div>Name in UCS</div>
</th>
<th tabindex="0" colspan="1" scope="col" data-column="2">
<div>Type</div>
</th>
<th tabindex="0" scope="col" data-column="3">
<div>Purpose</div>
</th>
<th tabindex="0" colspan="1" scope="col" data-column="4">
<div>Default value</div>
</th>
</tr>
</thead>
<tbody>
<tr>
<td>Flight speed</td>
<td colspan="1">speed</td>
<td colspan="1">Double</td>
<td>Flight speed</td>
<td colspan="1">
<p>The value corresponds to the value specified for the previous segment of the route.</p>
<p>If there are no segments, the value is taken from the profile = default ground speed.</p>
</td>
</tr>
<tr>
<td>Turn type</td>
<td colspan="1">wpTurnType</td>
<td colspan="1">Selection</td>
<td>
<p>Turn type. Can be one of the following values:</p>
<ul>
<li>STOP_AND_TURN;</li>
<li>STRAIGHT;</li>
<li>SPLINE;</li>
<li>BANK_TURN.</li>
</ul>
<p>When changing the machine, if selected value in the new list of allowed turn types, then it remains</p>
</td>
<td colspan="1">
<p>Turn type is selected from the list of available turn types for a profile.</p>
<p>The default value is the default value for the profile</p>
<p> </p>
</td>
</tr>
<tr>
<td colspan="1">Flight height</td>
<td colspan="1">height</td>
<td colspan="1">Double</td>
<td colspan="1">Flight height</td>
<td colspan="1"></td>
</tr>
<tr>
<td colspan="1">Height is above ground level</td>
<td colspan="1">heightAgl</td>
<td colspan="1">Boolean</td>
<td colspan="1">Height above ground level</td>
<td colspan="1">yes</td>
</tr>
<tr>
<td colspan="1">Number of laps</td>
<td colspan="1">loops</td>
<td colspan="1">Integer</td>
<td colspan="1">Number of repeated flights</td>
<td colspan="1">1</td>
</tr>
<tr>
<td colspan="1">Action execution</td>
<td colspan="1">actionExecution</td>
<td colspan="1">Selection</td>
<td colspan="1">
<p>Action execution. Shows when actions are to be performed:</p>
<ul>
<li>ONLY_AT_START - only at the starting point;</li>
<li>ACTIONS_EVERY_POINT - at each point.</li>
</ul>
</td>
<td colspan="1">Every point</td>
</tr>
<tr>
<td colspan="1">Avoid obstacles</td>
<td colspan="1">avoidObstacles</td>
<td colspan="1">Boolean</td>
<td colspan="1">Avoid obstacles</td>
<td colspan="1">yes</td>
</tr>
<tr>
<td colspan="1">Avoid terrain</td>
<td colspan="1">avoidTerrain</td>
<td colspan="1">Boolean</td>
<td colspan="1">Avoid terrain</td>
<td colspan="1">yes</td>
</tr>
<tr>
<td colspan="1">No actions at last point</td>
<td colspan="1">noActionsAtLastPoint</td>
<td colspan="1">Boolean</td>
<td colspan="1">Do not perform actions at the last point</td>
<td colspan="1">yes</td>
</tr>
</tbody>
</table>

<a name="photogrammetrytool"></a><h2>Photogrammetry tool</h2>
<table>
<thead>
<tr>
<th tabindex="0" scope="col" data-column="0">
<div>Name</div>
</th>
<th tabindex="0" colspan="1" scope="col" data-column="1">
<div>Name in UCS</div>
</th>
<th tabindex="0" colspan="1" scope="col" data-column="2">
<div>Type</div>
</th>
<th tabindex="0" scope="col" data-column="3">
<div>Purpose</div>
</th>
<th tabindex="0" colspan="1" scope="col" data-column="4">
<div>Default value</div>
</th>
<th tabindex="0" colspan="1" scope="col" data-column="5">
<div>Is the last value storing?</div>
</th>
</tr>
</thead>
<tbody>
<tr>
<td>Flight speed</td>
<td colspan="1">speed</td>
<td colspan="1">Double</td>
<td>Flight speed</td>
<td colspan="1">
<p>The value corresponds to the value specified for the previous segment of the route.</p>
<p>If there are no segments, the value is taken from the profile = default ground speed.</p>
</td>
<td colspan="1">no</td>
</tr>
<tr>
<td>Turn type</td>
<td colspan="1">wpTurnType</td>
<td colspan="1">Selection</td>
<td>
<p>Turn type. Can be one of the following values:</p>
<ul>
<li>STOP_AND_TURN;</li>
<li>STRAIGHT;</li>
<li>SPLINE;</li>
<li>BANK_TURN.</li>
</ul>
<p>When changing the machine, if selected value in the new list of allowed turn types, then it remains</p>
</td>
<td colspan="1">
<p>Turn type is selected from the list of available turn types for a profile.</p>
<p>The default value is the default value for the profile</p>
</td>
<td colspan="1">yes (from 2.12 per profile)</td>
</tr>
<tr>
<td>Camera</td>
<td colspan="1">camera</td>
<td colspan="1">Selection</td>
<td>
<p>Camera</p>
<p>List of identifier and name of cameras</p>
</td>
<td colspan="1">Select the camera available for this profile.</td>
<td colspan="1">yes</td>
</tr>
<tr>
<td colspan="1">Ground resolution, GSD</td>
<td colspan="1">groundSampleDistance</td>
<td colspan="1">Double</td>
<td colspan="1">
<p>Ground resolution, GSD.</p>
<p>The distance, which is placed in 1 pixel. By this value, the required height is calculated.</p>
</td>
<td colspan="1">null</td>
<td colspan="1">yes</td>
</tr>
<tr>
<td colspan="1">Forward overlap, %</td>
<td colspan="1">overlapForward</td>
<td colspan="1">Double</td>
<td colspan="1">
<p>Forward overlap, %.</p>
<p>Look at picture under the table</p>
</td>
<td colspan="1">60</td>
<td colspan="1">yes</td>
</tr>
<tr>
<td colspan="1">Side overlap, %</td>
<td colspan="1">overlapSide</td>
<td colspan="1">Double</td>
<td colspan="1">
<p>Side overlap, %</p>
<p>Look at picture under the table (lateral overlap)</p>
</td>
<td colspan="1">30</td>
<td colspan="1">yes</td>
</tr>
<tr>
<td colspan="1">Camera top facing forward</td>
<td colspan="1">camTopFacingForward</td>
<td colspan="1">Boolean</td>
<td colspan="1">Camera top facing forward</td>
<td colspan="1">yes</td>
<td colspan="1">yes</td>
</tr>
<tr>
<td colspan="1">Direction angle (0-360)</td>
<td colspan="1">directionAngle</td>
<td colspan="1">Double</td>
<td colspan="1">
<p>Direction angle (0-360)</p>
<p>Angle of rotation of the grid.</p>
</td>
<td colspan="1">0</td>
<td colspan="1">yes</td>
</tr>
<tr>
<td colspan="1">Avoid obstacles</td>
<td colspan="1">avoidObstacles</td>
<td colspan="1">Boolean</td>
<td colspan="1">Avoid obstacles</td>
<td colspan="1">yes</td>
<td colspan="1">yes</td>
</tr>
<tr>
<td colspan="1">Action execution</td>
<td colspan="1">actionExecution</td>
<td colspan="1">Selection</td>
<td colspan="1">
<p>Action execution. Shows when actions are to be performed:</p>
<ul>
<li>ONLY_AT_START - only at the starting point;</li>
<li>ACTIONS_EVERY_POINT - at each point;</li>
<li>ACTIONS_ON_FORWARD_PASSES - at each point of passes. But when turning, the camera should turn off.</li>
</ul>
</td>
<td colspan="1">every point</td>
<td colspan="1">yes</td>
</tr>
<tr>
<td colspan="1">Additional waypoints</td>
<td colspan="1">generateAdditionalWaypoints</td>
<td colspan="1">Boolean</td>
<td colspan="1">
<p>Additional waypoints</p>
<p>Is place additional snake points apart from the side points.</p>
</td>
<td colspan="1">no</td>
<td colspan="1">yes</td>
</tr>
<tr>
<td colspan="1">Overshoot</td>
<td colspan="1">overshoot</td>
<td colspan="1">Double</td>
<td colspan="1">
<p>Overshoot</p>
<p>Additional distance beyond the snake, which must be passed to properly and completely pass through the snake taking into account the features of the rotation of the plane.</p>
</td>
<td colspan="1">null</td>
<td colspan="1">yes</td>
</tr>
<tr>
<td colspan="1">Overshoot speed</td>
<td colspan="1">overshootSpeed</td>
<td colspan="1">Double</td>
<td colspan="1">Speed for overshoot part</td>
<td colspan="1">null</td>
<td colspan="1">yes</td>
</tr>
<tr>
<td colspan="1">Altitude type</td>
<td colspan="1">altitudeType</td>
<td colspan="1">Selection</td>
<td colspan="1">
<p>Usage altitude type:</p>
<ul>
<li>WGS84 - route traversing one given AMSL-altitude;</li>
<li>AGL - route traversing one given AGL-altitude. In this case, the passage occurs along the height relative to the lower point, and the distance between the passes is calculated so as to cover the entire surface at the highest and lowest points.</li>
</ul>
</td>
<td colspan="1">AMSL</td>
<td colspan="1">yes</td>
</tr>
<tr>
<td colspan="1">Allow partial calculation</td>
<td colspan="1">areaScanAllowPartialCalculation</td>
<td colspan="1">Boolean</td>
<td colspan="1">Whether to allow partial path computation or to throw an error when some of the points are inaccessible for some reason</td>
<td colspan="1">no</td>
<td colspan="1">yes</td>
</tr>
<tr>
<td colspan="1">AGL Tolerance</td>
<td colspan="1">tolerance</td>
<td colspan="1">Double</td>
<td colspan="1">
<p>Allowable height difference, when you can not put additional points. An additional point is placed if it goes beyond this boundary.</p>
<p>If not set should be treated as 0 (zero)</p>
</td>
<td colspan="1">3</td>
<td colspan="1">yes</td>
</tr>
<tr>
<td colspan="1">No actions at last point</td>
<td colspan="1">noActionsAtLastPoint</td>
<td colspan="1">Boolean</td>
<td colspan="1">Do not perform actions at the last point</td>
<td colspan="1">yes</td>
<td colspan="1"> </td>
</tr>
</tbody>
</table>

<a name="takeoff"></a><h2>Takeoff</h2>
<table>
<thead>
<tr>
<th tabindex="0" scope="col" data-column="0">
<div>Name</div>
</th>
<th tabindex="0" colspan="1" scope="col" data-column="1">
<div>Name in UCS</div>
</th>
<th tabindex="0" colspan="1" scope="col" data-column="2">
<div>Type</div>
</th>
<th tabindex="0" scope="col" data-column="3">
<div>Purpose</div>
</th>
<th tabindex="0" colspan="1" scope="col" data-column="4">
<div>Default value</div>
</th>
</tr>
</thead>
<tbody>
<tr>
<td>Flight speed</td>
<td colspan="1">speed</td>
<td colspan="1">Double</td>
<td>Flight speed</td>
<td colspan="1">
<p>The value corresponds to the value specified for the previous segment of the route.</p>
<p>If there are no segments, the value is taken from the profile = default ground speed.</p>
</td>
</tr>
<tr>
<td colspan="1">Avoid obstacles</td>
<td colspan="1">avoidObstacles</td>
<td colspan="1">Boolean</td>
<td colspan="1">Avoid obstacles</td>
<td colspan="1">yes</td>
</tr>
<tr>
<td colspan="1">Avoid terrain</td>
<td colspan="1">avoidTerrain</td>
<td colspan="1">Boolean</td>
<td colspan="1">Avoid terrain</td>
<td colspan="1">yes</td>
</tr>
</tbody>
</table>

<a name="waypoint"></a><h2>Waypoint</h2>
<table>
<thead>
<tr>
<th tabindex="0" scope="col" data-column="0">
<div>Name</div>
</th>
<th tabindex="0" colspan="1" scope="col" data-column="1">
<div>Name in UCS</div>
</th>
<th tabindex="0" colspan="1" scope="col" data-column="2">
<div>Type</div>
</th>
<th tabindex="0" scope="col" data-column="3">
<div>Purpose</div>
</th>
<th tabindex="0" colspan="1" scope="col" data-column="4">
<div>Default value</div>
</th>
</tr>
</thead>
<tbody>
<tr>
<td>Flight speed</td>
<td colspan="1">speed</td>
<td colspan="1">Double</td>
<td>Flight speed</td>
<td colspan="1">
<p>The value corresponds to the value specified for the previous segment of the route.</p>
<p>If there are no segments, the value is taken from the profile = default ground speed.</p>
</td>
</tr>
<tr>
<td>Turn type</td>
<td colspan="1">wpTurnType</td>
<td colspan="1">Selection</td>
<td>
<p>Turn type. Can be one of the following values:</p>
<ul>
<li>STOP_AND_TURN;</li>
<li>STRAIGHT;</li>
<li>SPLINE;</li>
<li>BANK_TURN.</li>
</ul>
<p>When changing the machine, if selected value in the new list of allowed turn types, then it remains</p>
</td>
<td colspan="1">
<p>Turn type is selected from the list of available turn types for a profile.</p>
<p>The default value is the default value for the profile</p>
</td>
</tr>
<tr>
<td colspan="1">Avoid obstacles</td>
<td colspan="1">avoidObstacles</td>
<td colspan="1">Boolean</td>
<td colspan="1">Avoid obstacles</td>
<td colspan="1">yes</td>
</tr>
<tr>
<td colspan="1">Avoid terrain</td>
<td colspan="1">avoidTerrain</td>
<td colspan="1">Boolean</td>
<td colspan="1">Avoid terrain</td>
<td colspan="1">yes</td>
</tr>
</tbody>
</table>