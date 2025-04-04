Hello, and thank you for purchasing my package from the Unity asset store.

Inside this package includes the necessary tools for you to easily add a healthbar to any object in your game with minimal setup.

The healthbar prefabs are located in the folder "../Nick's Assets/Nick's Healthbar/Healthbars". You can preview the different healthbars by double clicking them inside the unity editor.

SCRIPTS :
----------------------------
All scripts related to Healthbar's are located in the folder "../Nick's Assets/Nick's Healthbar/Scripts" 

The HealthBar script contains the code necessary to instantiate the Healthbar, and apply the effect of taking damage. It also adds the ability to play a sound when do damage to the healthbar, and play an animation when you do damage to the healthbar. It has many comments that can help you understand how to read the code and all of the fields located under the HealthBar script in the inspector.

The Billboard script allows the Healthbar prefabs themselves to act as a "Billboard", meaning all Healthbar's with this script attached will constantly look at the MAIN CAMERA, they will not look at any other cam. The script itself is much shorter and contains no comments.
----------------------------
The setup goes as follows :
----------------------------
Step 1 : Create the object that you want to have a healthbar (if you already have this object created just make sure it is selected in the unity editor)

Step 2 : Scroll down until you see the "Add component" button in the inspector.

Step 3 : Click the "Add component" button and in the search bar type in "Health Bar"

Step 4 : A C# script should appear, double click it to add it to the object selected

Step 5 : Click on the little circle icon next to the "Healthbar" field under the Health Bar component. A window should pop up and have two tabs "Scene" and "Assets"

Step 6 : Make sure the Assets tab is active and you should see 5 objects named (You might have to search for them in the search bar) ,"Healthbar1", "Healthbar2", "Healthbar3", "Healthbar4", and "Healthbar5.

Step 7 : Click on any one of them and make sure that the Healthbar you selected from that window is in the "Healthbar" field under the "Health Bar" component.

Step 8 : Click play and the object that you added the Health bar to should now have a health bar above it. You may notice it is stretched.

Step 9 : Because unity requires all prefabs uploaded to the asset store to have a rotation of (0,0,0) and a scale of (1,1,1) I was forced into stretching them. You can fix this by double clicking one of the Healthbar prefabs inside the unity editor (See the top of this documentation for information on where they are located). Once you double click them you should see the healthbar in the scene view. 

Step 10 : Once you have the Healthbar prefab in the scene view, feel free to scale it to however you want.

Step 11 : Click play and see if the healthbar is scaled properly

Step 12 : Add any sound clips if you want to to play when the healthbar takes damage.

Step 13 : Enable the animation if you want, and customize the values in the inspector. Once you are done, the setup is complete and you can use the healthbars!
----------------------------
NOTE : 
----------------------------
In order to do damage to the Healthbar's you are going to have to call the method TakeDamage(amount) contained in the Health Bar class. You can see how this is done by viewing the "DecreaseTestHealth" script located at "../Nick's Assets/ Nick's Healthbar/Demo scene/Test scene scripts" and reading the RayCastMethod() function/method.

If you don't want to play any sound when the healthbar takes damage, just leave the "Hit sound" field blank..

The Healthbar's do automatically have a billboard script attached on them. This script will only work properly if the scene has one camera, and it is tagged as the main camera. 

You can delete the Billboard class however be warned that the Healthbar will face the object it is attached to's forward direction.
----------------------------
CUSTOMIZATION :
----------------------------
You can customize the Healthbar's and create your own, you'll have to create 3 images, One image for the Healthbar's background, one image for how the Healthbar will look when it is full, and one more image for the Effect of taking damage. You may be confused on the last one, but just copy the Healthbar's full image, and instead of whatever you chose for the color of the Healthbar when it is full, instead choose a different, solid color. For many of the Healthbar's included it is red. 

Once you have created the three images, select them one at a time in the unity editor, and in the inspector make sure the "Texture Type" field is set to "Sprite (2D and UI)". After this, go and make a duplicate of one of the already created Healthbar prefabs and name it whatever you'd like. 

Then double click on your newly created Healthbar prefab, and you should see it in the scene view. But you may notice it looks just like the Healthbar you duplicated, this is because you have not actually used any of the Images you created for your new Healthbar yet. So to fix that, click on the "HealthBarHealth" object in the hierarchy and in the "Source Image" field in the inspector under the Image component. Now you should see a window, in that window make sure the Assets tab is active and select the image you created that is for how the Healthbar will look when it is full. 

Repeat the last step for each object under the Canvas object in the hierarchy, but :
	For "HealthBarBack" select the image for the background of your Healthbar.
	For "mask" select the image for the damage effect of your Healthbar.
	For "HealthBarHide" select the image for the damage effect of your Healthbar.
----------------------------

And that's it! Please leave a review on this package on the unity asset store, any feedback is appreciated! 
