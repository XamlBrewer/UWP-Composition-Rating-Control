# UWP-Composition-Rating-Control
A UWP Rating Control drawn by the Composition API.

Comes with the following dependency properties:
* Maximum (int): number of stars, maximum score
* StepFrequency (double): rounding interval, a percentage (e.g. 0.25)
* Value (double): current value (from 0 to Maximum)
* ItemHeight (int): height (and width) of each image in device independent pixels
* ImagePadding (int): pixels between images
* FilledImage (urn): path to the filled image
* EmptyImage (urn): path to the empty image
* IsInteractive (bool): whether or not the control responds to user input (tapping or sliding)

Behavior:
* Tap on an image to apply the integral value
* Slide horizontally over the control to decrease and increase value with StepFrequency
