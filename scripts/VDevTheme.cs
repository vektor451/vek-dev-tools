// This code is horrid.
using Godot;
using System;

public partial class VDevTheme : Theme
{
    [Export(PropertyHint.File)] string closeSVGPath;

    public float scaleFactor = 2.0f;

    int defFontSize = 12;
    int defTitleHeight = 24;
    int defResizeMargin = 4;
    int defCloseHOffset = 16;
    int defCloseVOffset = 16;

    StyleBoxFlat defWindowStyleBox;
    StyleBoxFlat defLineEditStyleBox;
    StyleBoxFlat defLabelStyleBox;

	StyleBoxFlat defVScrollBarGrabber;
	StyleBoxFlat defVScrollBarGrabberHighlight;
	StyleBoxFlat defVScrollBarGrabberPressed;
	StyleBoxFlat defVScrollBarScroll;
	StyleBoxFlat defVScrollBarScrollFocus;

    public void Init()
	{
		defFontSize 	= DefaultFontSize;
		defTitleHeight 	= (int)Get("Window/constants/title_height");
		defResizeMargin = (int)Get("Window/constants/resize_margin");
		defCloseHOffset = (int)Get("Window/constants/close_h_offset");
		defCloseVOffset = (int)Get("Window/constants/close_v_offset");

		// evil double cast because godot
		defWindowStyleBox 	= ((StyleBoxFlat)Get("Window/styles/embedded_border")).Duplicate() as StyleBoxFlat;
		defLineEditStyleBox = ((StyleBoxFlat)Get("LineEdit/styles/normal")).Duplicate() as StyleBoxFlat;
		defLabelStyleBox 	= ((StyleBoxFlat)Get("Label/styles/normal")).Duplicate() as StyleBoxFlat;
		
		defVScrollBarGrabber 			= ((StyleBoxFlat)Get("VScrollBar/styles/grabber")).Duplicate() as StyleBoxFlat;
		defVScrollBarGrabberHighlight 	= ((StyleBoxFlat)Get("VScrollBar/styles/grabber_highlight")).Duplicate() as StyleBoxFlat;
		defVScrollBarGrabberPressed 	= ((StyleBoxFlat)Get("VScrollBar/styles/grabber_pressed")).Duplicate() as StyleBoxFlat;
		defVScrollBarScroll 			= ((StyleBoxFlat)Get("VScrollBar/styles/scroll")).Duplicate() as StyleBoxFlat;

		UpdateScale();
	}

	public void UpdateScale()
	{
		DefaultFontSize = Mathf.RoundToInt(defFontSize * scaleFactor);
		Set("Window/constants/title_height", Mathf.Round(defTitleHeight * scaleFactor));
		Set("Window/constants/resize_margin", Mathf.Round(defResizeMargin * scaleFactor));
		Set("Window/constants/close_h_offset", Mathf.Round(defCloseHOffset * scaleFactor));
		Set("Window/constants/close_v_offset", Mathf.Round(defCloseVOffset * scaleFactor));

		FileAccess svg = FileAccess.Open(closeSVGPath, FileAccess.ModeFlags.Read);

		Image closeImg = new();
		closeImg.LoadSvgFromBuffer(svg.GetBuffer((long)svg.GetLength()), scaleFactor);

		ImageTexture closeTex = new();
		closeTex.SetImage(closeImg);

		Set("Window/icons/close", closeTex);
		Set("Window/icons/close_pressed", closeTex);

		StyleBoxFlat winStyleBox 		= (StyleBoxFlat)Get("Window/styles/embedded_border");
		StyleBoxFlat lineEditStyleBox 	= (StyleBoxFlat)Get("LineEdit/styles/normal");
		StyleBoxFlat labelStyleBox 		= (StyleBoxFlat)Get("Label/styles/normal");

		StyleBoxFlat vScrollBarGrabber 			= (StyleBoxFlat)Get("VScrollBar/styles/grabber");
		StyleBoxFlat vScrollBarGrabberHighlight = (StyleBoxFlat)Get("VScrollBar/styles/grabber_highlight");
		StyleBoxFlat vScrollBarGrabberPressed 	= (StyleBoxFlat)Get("VScrollBar/styles/grabber_pressed");
		StyleBoxFlat vScrollBarScroll 			= (StyleBoxFlat)Get("VScrollBar/styles/scroll");

		UpdateExpandMargin(ref defWindowStyleBox, ref winStyleBox);
		UpdateBorderWidth(ref defWindowStyleBox, ref winStyleBox);
		UpdateCornerRadius(ref defWindowStyleBox, ref winStyleBox);
		
		UpdateBorderWidth(ref defLineEditStyleBox, ref lineEditStyleBox);
        UpdateContentMargin(ref defLineEditStyleBox, ref lineEditStyleBox);
		
        UpdateContentMargin(ref defLabelStyleBox, ref labelStyleBox);
		UpdateCornerRadius(ref defLabelStyleBox, ref labelStyleBox);
		
		
		UpdateContentMargin(ref defVScrollBarGrabber, ref vScrollBarGrabber);
		UpdateCornerRadius(ref defVScrollBarGrabber, ref vScrollBarGrabber);

		UpdateContentMargin(ref defVScrollBarGrabberHighlight, ref vScrollBarGrabberHighlight);
		UpdateCornerRadius(ref defVScrollBarGrabberHighlight, ref vScrollBarGrabberHighlight);

		UpdateContentMargin(ref defVScrollBarGrabberPressed, ref vScrollBarGrabberPressed);
		UpdateCornerRadius(ref defVScrollBarGrabberPressed, ref vScrollBarGrabberPressed);

		UpdateContentMargin(ref defVScrollBarScroll, ref vScrollBarScroll);
		UpdateCornerRadius(ref defVScrollBarScroll, ref vScrollBarScroll);
		
    }

    public void UpdateExpandMargin(ref StyleBoxFlat defStyle, ref StyleBoxFlat currentStyle)
    {
        currentStyle.ExpandMarginBottom = Mathf.RoundToInt(defStyle.ExpandMarginBottom * scaleFactor);
        currentStyle.ExpandMarginTop    = Mathf.RoundToInt(defStyle.ExpandMarginTop * scaleFactor);
        currentStyle.ExpandMarginLeft 	= Mathf.RoundToInt(defStyle.ExpandMarginLeft * scaleFactor);
        currentStyle.ExpandMarginRight 	= Mathf.RoundToInt(defStyle.ExpandMarginRight * scaleFactor);
    }

    public void UpdateContentMargin(ref StyleBoxFlat defStyle, ref StyleBoxFlat currentStyle)
    {
        currentStyle.ContentMarginBottom    = Mathf.RoundToInt(defStyle.ContentMarginBottom * scaleFactor);
        currentStyle.ContentMarginTop       = Mathf.RoundToInt(defStyle.ContentMarginTop * scaleFactor);
        currentStyle.ContentMarginLeft      = Mathf.RoundToInt(defStyle.ContentMarginLeft * scaleFactor);
        currentStyle.ContentMarginRight     = Mathf.RoundToInt(defStyle.ContentMarginRight * scaleFactor);
    }

    public void UpdateBorderWidth(ref StyleBoxFlat defStyle, ref StyleBoxFlat currentStyle)
    {
        currentStyle.BorderWidthBottom 	= Mathf.RoundToInt(defStyle.BorderWidthBottom * scaleFactor);
        currentStyle.BorderWidthTop 	= Mathf.RoundToInt(defStyle.BorderWidthTop * scaleFactor);
        currentStyle.BorderWidthLeft 	= Mathf.RoundToInt(defStyle.BorderWidthLeft * scaleFactor);
        currentStyle.BorderWidthRight 	= Mathf.RoundToInt(defStyle.BorderWidthRight * scaleFactor);
    }

    public void UpdateCornerRadius(ref StyleBoxFlat defStyle, ref StyleBoxFlat currentStyle)
    {
        currentStyle.CornerRadiusBottomLeft 	= Mathf.RoundToInt(defStyle.CornerRadiusBottomLeft * scaleFactor);
        currentStyle.CornerRadiusBottomRight 	= Mathf.RoundToInt(defStyle.CornerRadiusBottomRight * scaleFactor);
        currentStyle.CornerRadiusTopLeft 		= Mathf.RoundToInt(defStyle.CornerRadiusTopLeft * scaleFactor);
        currentStyle.CornerRadiusTopRight 		= Mathf.RoundToInt(defStyle.CornerRadiusTopRight * scaleFactor);
    }
    
    
}
