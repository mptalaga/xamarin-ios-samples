// This file has been autogenerated from a class added in the UI designer.

using System;

using Foundation;
using UIKit;

namespace Todo11App
{
    public partial class DetailViewController : UIViewController
	{
		public TodoItem Current {get;set;}
		
        public TableViewController Delegate { get; set; }

		public DetailViewController (IntPtr handle) : base (handle)
		{
		}
        UIButton PhotoButton;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
            // DEMO: the second item in the stack disables large titles
            NavigationItem.LargeTitleDisplayMode = UINavigationItemLargeTitleDisplayMode.Never;

			SaveButton.TouchUpInside += (sender, e) => {
				
				Current.Name = NameText.Text;
				Current.Notes = NotesText.Text;
				Current.Done = DoneSwitch.On;

				// includes CoreSpotlight indexing!
				Delegate.SaveTodo(Current); 

				UIAccessibility.PostNotification (UIAccessibilityPostNotification.Announcement, new NSString(@"Item was saved"));

				NavigationController.PopViewController(true);
			};
			CancelButton.TouchUpInside += (sender, e) => {
				if (Delegate != null) 
				{
					Delegate.DeleteTodo(Current); // also CoreSpotlight

					UIAccessibility.PostNotification (UIAccessibilityPostNotification.Announcement,new NSString(@"Item was deleted"));
				}
				else 
                {
					Console.WriteLine("Delegate not set - this shouldn't happen");
				}

				NavigationController.PopViewController(true);
			};

			NameText.TextAlignment = UITextAlignment.Natural;
			NotesText.TextAlignment = UITextAlignment.Natural;

			UserActivity = UserActivityHelper.CreateNSUserActivity (Current?? new TodoItem());

            // Map button
            PhotoButton = UIButton.FromType(UIButtonType.Custom);
            PhotoButton.SetTitle("Photo", UIControlState.Normal);
            PhotoButton.BackgroundColor = UIColor.Green;

            PhotoButton.SizeToFit();
            PhotoButton.TouchUpInside += (sender, e) => {
                Console.WriteLine("take photo");
                var popover = Storyboard.InstantiateViewController("photo");

                Console.WriteLine("pass todo item");
                (popover as PhotoViewController).Todo = (NavigationController.VisibleViewController as DetailViewController).Current;

                PresentViewController(popover, true, null);

                // Configure the popover for the iPad, the popover displays as a modal view on the iPhone
                UIPopoverPresentationController presentationPopover = popover.PopoverPresentationController;
                if (presentationPopover != null)
                {
                    presentationPopover.SourceView = View;
                    presentationPopover.PermittedArrowDirections = UIPopoverArrowDirection.Up;
                    presentationPopover.SourceRect = View.Frame;
                }
            };
            View.AddSubview(PhotoButton);
		}
        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();

            PhotoButton.Layer.CornerRadius = PhotoButton.Layer.Frame.Size.Width / 2;
            PhotoButton.BackgroundColor = UIColor.FromRGB(0x5A, 0x86, 0x22); // 5A8622 dark-green
            PhotoButton.SetTitleColor(UIColor.FromRGB(0xCF, 0xEF, 0xa7), UIControlState.Normal); // CFEFA7 light-green
            PhotoButton.ClipsToBounds = true;
            //PhotoButton.setImage(UIImage(named: "your-image"), for: .normal)
            PhotoButton.TranslatesAutoresizingMaskIntoConstraints = false;

            var safeGuide = View.SafeAreaLayoutGuide;
            NSLayoutConstraint.ActivateConstraints(new NSLayoutConstraint[] {
                PhotoButton.TrailingAnchor.ConstraintEqualTo(safeGuide.TrailingAnchor, -23 - 60 - 23),
                PhotoButton.BottomAnchor.ConstraintEqualTo(safeGuide.BottomAnchor, -13),
                PhotoButton.WidthAnchor.ConstraintEqualTo(60),
                PhotoButton.HeightAnchor.ConstraintEqualTo(60)
            });

            var wideGuide = View.LayoutMarginsGuide;
            NSLayoutConstraint.ActivateConstraints(new NSLayoutConstraint[] {
                Photo.TopAnchor.ConstraintEqualTo(SaveButton.BottomAnchor, 23),
                Photo.LeadingAnchor.ConstraintEqualTo(wideGuide.LeadingAnchor),
                Photo.TrailingAnchor.ConstraintEqualTo(wideGuide.TrailingAnchor),
                Photo.BottomAnchor.ConstraintEqualTo(wideGuide.BottomAnchor),
            });
        }
		public override void ViewWillDisappear (bool animated)
		{
			UserActivity?.ResignCurrent ();

			base.ViewWillDisappear (animated);
		}
		// when displaying, set-up the properties
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			NameText.Text = Current.Name;
			NotesText.Text = Current.Notes;
			DoneSwitch.On = Current.Done;

            if (Current.HasImage)
            {
                var documentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                string jpgFilename = System.IO.Path.Combine(documentsDirectory, Current.Id + ".jpg");
                Photo.Image = UIImage.FromFile(jpgFilename);
            }
			// button is cancel or delete
			if (Current.Id > 0) {
				CancelButton.SetTitle (NSBundle.MainBundle.LocalizedString ("Delete", "")
					, UIControlState.Normal);
				CancelButton.SetTitleColor (UIColor.Red, UIControlState.Normal);
			} else {
				CancelButton.SetTitle(NSBundle.MainBundle.LocalizedString ("Cancel", "")
					,UIControlState.Normal);
				CancelButton.SetTitleColor (UIColor.DarkTextColor, UIControlState.Normal);
			}
		}


		// this will be called before the view is displayed 
		public void SetTodo (TodoItem todo) {
			Current = todo;
		}
	}
}
