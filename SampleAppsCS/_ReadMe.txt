READ ME

last updated: JP (9/14/2019)


--------------------------------------------------------------- DOXYGEN ---------------------------------------------------------------
------------------------------------------------- HOW TO PUBLISH SAMPLE APPLICATIONS  -------------------------------------------------

FILES NEEDED:

1. _rsi.h                            - You specify the tree hierarchy of the pages or groups you are adding
2. rsi_layout.xml                    - You can change the order of the nav tree and the name of the item (groups, classes, examples, etc)
3. your sample app .cpp or .cpp        - You include link them to the page added in _rsi.h


METHODS:

1. Using @page and @subpage tags.
2. Using @examples tag.


================================[1]==============================
================= USING @page and @subpage TAGS =================
=================================================================

*** The Not as Simple but Clean method.


1. Go to: RSIQDNET4 > "_rsi.h" and add a new subpage under the correct page (cpp or cpp)
-----------------------------------------------------------------

/*!  \page sampleapps Sample Apps

Welcome, explore our sample applications.

We offer sample apps in the following languages:
- \subpage cpp
- \subpage cpp "C++"
*/

//-----------------------------------------------------------

/*!  \page cpp C#
Our C# Sample Apps:

- \subpage absolute-motion-cpp                            - Move a single axis in trapezoidal profile to an absolute distance.
- \subpage axis-settling-cpp                                - Set the settling time of an axis.


*/

//-----------------------------------------------------------

/*!  \page cpp C++
Our C++ Sample Apps:

- \subpage absolute-motion-cpp                            - Move a single axis in trapezoidal profile to an absolute distance. 
- \subpage capture-cpp                                    - etc...

*/


2. Go to: "YourSampleApp.cpp or YourSampleApp.cpp" and add these tags.
-----------------------------------------------------------------

EXAMPLE:

/*!  
*  @page       absolute-motion-cpp AbsoluteMotion.cpp

*  @brief      Absolute Motion sample application.

*  @details    This sample application moves a single axis in trapezoidal profile to an absolute distance set by RELATIVE_POSITION below.

*  @pre        This sample code presumes that the user has set the tuning paramters(PID, PIV, etc.) prior to running this program so that the motor can rotate in a stable manner.

*  @warning    This is a sample program to assist in the integration of your motion controller with your application. It may not contain all of the logic and safety features that your application requires.

*  @copyright
Copyright(c) 1998-2019 by Robotic Systems Integration, Inc. All rights reserved.
This software contains proprietary and confidential information of Robotic 
Systems Integration, Inc. (RSI) and its suppliers. Except as may be set forth 
in the license agreement under which this software is supplied, disclosure, 
reproduction, or use with controls other than those provided by RSI or suppliers
for RSI is strictly prohibited without the prior express written consent of 
Robotic Systems Integration.
* 
* @include AbsoluteMotion.cpp
*/


3. NOT NECESSARY - if you wish to change the location of sample
apps in the nav bar, go to rsi_layout.xml
-----------------------------------------------------------------

4. Make sure to add 
../../RapidStuff/SampleAppscpp/
../../RapidStuff/SampleAppsCPP/

to EXAMPLE_PATH in your doxyfile
-----------------------------------------------------------------



================================[2]==============================
====================== USING @examples TAG ======================
=================================================================

*** The Simple but not Clean method.



1. Go to: "YourSampleApp.cpp or YourSampleApp.cpp" and add these tags.
-----------------------------------------------------------------

EXAMPLE:

/*!  
*  @example    Camming.cpp

*  @brief      Camming sample application.

*  @details    Master-slave motion.

*  @pre        This sample code presumes that the user has set the tuning paramters(PID, PIV, etc.) prior to running this program so that the motor can rotate in a stable manner.

*  @warning    This is a sample program to assist in the integration of your motion controller with your application. It may not contain all of the logic and safety features that your application requires.

*  @copyright 
Copyright(c) 1998-2019 by Robotic Systems Integration, Inc. All rights reserved.
This software contains proprietary and confidential information of Robotic 
Systems Integration, Inc. (RSI) and its suppliers. Except as may be set forth 
in the license agreement under which this software is supplied, disclosure, 
reproduction, or use with controls other than those provided by RSI or suppliers
for RSI is strictly prohibited without the prior express written consent of 
Robotic Systems Integration.

*  @section overview_sec Overview 
This sample application allows you to command a nonlinear coordinated motion betweentwo axes. 

Master Axis: this axis/motor may or may not be controlled by the motion controller. 
Slave Axis: the motion controller controls the position of this axis/motor as a function of the position of the master axis.


NOTE: User Units must be set to 1! 
*/

3. NOT NECESSARY - if you wish to change the location or name of Sample
Apps in the nav bar, go to rsi_layout.xml
-----------------------------------------------------------------


============================== TAGS =============================


@ref         -    allows you to insert hyperlinks.
@example     -   allows you to add a sample app to the "Examples" group.
@page       -    allows you to create your own markdown or txt page.
@brief      -    allows you to insert info.
@details    -    allows you to insert more info.
@pre        -      allows you to insert a preconditions block.
@warning    -    allows you to add a warning block.
@copyright    -    allows you to a copyright block.
@include     -   needed to include code block on @pages. (see above for example)



