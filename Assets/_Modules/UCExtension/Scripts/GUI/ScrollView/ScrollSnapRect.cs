using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;


namespace UCExtension.GUI
{

    [RequireComponent(typeof(Image))]
    //[RequireComponent(typeof(RectMask2D))]
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollSnapRect : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public static ScrollSnapRect Ins;
        [Tooltip("Set starting page index - starting from 0")]
        public int startingPage = 0;
        [Tooltip("Threshold time for fast swipe in seconds")]
        public float fastSwipeThresholdTime = 0.3f;
        [Tooltip("Threshold time for fast swipe in (unscaled) pixels")]
        public int fastSwipeThresholdDistance = 100;
        [Tooltip("How fast will page lerp to target position")]
        public float decelerationRate = 10f;
        [Tooltip("Button to go to the previous page (optional)")]
        public Button prevButton;
        [Tooltip("Button to go to the next page (optional)")]
        public Button nextButton;
        [Tooltip("Sprite for unselected page (optional)")]
        public Sprite unselectedPage;
        [Tooltip("Sprite for selected page (optional)")]
        public Sprite selectedPage;
        [Tooltip("Container with page images (optional)")]
        public Transform pageSelectionIcons;
        [Tooltip("Container with page count (optional)")]
        public Text pageCountText;

        [SerializeField] bool unscaledTime;

        [SerializeField] bool hideTransitionButton;

        // fast swipes should be fast and short. If too long, then it is not fast swipe
        private int _fastSwipeThresholdMaxLimit;

        private ScrollRect _scrollRectComponent;
        private RectTransform _scrollRectRect;
        private RectTransform _container;
        public bool autoInit = false;
        private bool _horizontal;

        // number of pages in container
        private int _pageCount;
        private int _currentPage;

        // whether lerping is in progress and target lerp position
        private bool _lerp;
        private Vector2 _lerpTo;

        // target position of every page
        private List<Vector2> _pagePositions = new List<Vector2>();

        // in draggging, when dragging started and where it started
        private bool _dragging;
        private float _timeStamp;
        private Vector2 _startPosition;


        // for showing small page icons
        public bool _showPageSelection;
        private int _previousPageSelectionIndex;
        public Image pageSelectionPrefab;
        // container with Image components - one Image for each page
        List<Image> _pageSelectionImages = new List<Image>();
        public UnityAction<int> onPageChange;

        //------------------------------------------------------------------------
        private void Awake()
        {
            Ins = this;
            if (nextButton)
                nextButton.onClick.AddListener(() => { NextScreen(); });
            if (prevButton)
                prevButton.onClick.AddListener(() => { PreviousScreen(); });
        }
        void Start()
        {
            if (autoInit)
            {
                ResetPage();
            }
        }
        void OnEnable()
        {
            if (unscaledTime)
            {
                StopAllCoroutines();
                StartCoroutine(IEUpdate());
            }
        }
        IEnumerator IEUpdate()
        {
            float lastTime = Time.unscaledTime;
            while (true)
            {

                if (_lerp)
                {
                    // prevent overshooting with values greater than 1
                    float deltaTime = Time.unscaledTime - lastTime;
                    float decelerate = Mathf.Min(decelerationRate * deltaTime, 1f);
                    _container.anchoredPosition = Vector2.Lerp(_container.anchoredPosition, _lerpTo, decelerate);
                    // time to stop lerping?
                    if (Vector2.SqrMagnitude(_container.anchoredPosition - _lerpTo) < 0.25f)
                    {
                        // snap to target and stop lerping
                        _container.anchoredPosition = _lerpTo;
                        _lerp = false;
                        // clear also any scrollrect move that may interfere with our lerping
                        _scrollRectComponent.velocity = Vector2.zero;
                    }

                    // switches selection icon exactly to correct page
                    if (_showPageSelection)
                    {
                        SetPageSelection(GetNearestPage());
                    }
                }
                lastTime = Time.unscaledTime;
                yield return new WaitForEndOfFrame();
            }
        }
        public void GoToPage(int page)
        {
            SetHighPage(page);
        }

        public void ResetPage()
        {
            _scrollRectComponent = GetComponent<ScrollRect>();
            _scrollRectRect = GetComponent<RectTransform>();
            _container = _scrollRectComponent.content;
            _pageCount = _container.childCount;
            if (pageCountText) pageCountText.text = $"{startingPage + 1}/{_pageCount}";
            // is it horizontal or vertical scrollrect
            if (_scrollRectComponent.horizontal && !_scrollRectComponent.vertical)
            {
                _horizontal = true;
            }
            else if (!_scrollRectComponent.horizontal && _scrollRectComponent.vertical)
            {
                _horizontal = false;
            }
            else
            {
                Debug.LogWarning("Confusing setting of horizontal/vertical direction. Default set to horizontal.");
                _horizontal = true;
            }

            _lerp = false;

            // init
            SetPagePositions();
            SetPage(startingPage);
            if (_showPageSelection)
            {
                InitPageSelection(_pageCount);
                SetPageSelection(startingPage);
            }
            SetHighPage(startingPage);
            onPageChange?.Invoke(startingPage);
            // prev and next buttons
        }
        public void SetHighPage(int id)
        {
            if (_pageCount > 0)
            {
                SetPage(id);
                SetPageSelection(id);
            }
            else
            {
                startingPage = id;
            }
        }

        //------------------------------------------------------------------------
        void Update()
        {
            if (unscaledTime) return;
            // if moving to target position
            if (_lerp)
            {
                // prevent overshooting with values greater than 1
                float decelerate = Mathf.Min(decelerationRate * Time.deltaTime, 1f);
                _container.anchoredPosition = Vector2.Lerp(_container.anchoredPosition, _lerpTo, decelerate);
                // time to stop lerping?
                if (Vector2.SqrMagnitude(_container.anchoredPosition - _lerpTo) < 0.25f)
                {
                    // snap to target and stop lerping
                    _container.anchoredPosition = _lerpTo;
                    _lerp = false;
                    // clear also any scrollrect move that may interfere with our lerping
                    _scrollRectComponent.velocity = Vector2.zero;
                }

                // switches selection icon exactly to correct page
                if (_showPageSelection)
                {
                    SetPageSelection(GetNearestPage());
                }
            }
        }
        //------------------------------------------------------------------------
        private void SetPagePositions()
        {
            int width = 0;
            int height = 0;
            int offsetX = 0;
            int offsetY = 0;
            int containerWidth = 0;
            int containerHeight = 0;

            if (_horizontal)
            {
                // screen width in pixels of scrollrect window
                width = (int)_scrollRectRect.rect.width + 20;
                // center position of all pages
                offsetX = width / 2;
                // total width
                containerWidth = width * _pageCount;
                // limit fast swipe length - beyond this length it is fast swipe no more
                _fastSwipeThresholdMaxLimit = width;
            }
            else
            {

                height = (int)_scrollRectRect.rect.height;
                offsetY = height / 2;
                containerHeight = height * _pageCount;
                _fastSwipeThresholdMaxLimit = height;
            }

            // set width of container
            Vector2 newSize = new Vector2(containerWidth, containerHeight);
            _container.sizeDelta = newSize;
            Vector2 newPosition = new Vector2(containerWidth / 2, containerHeight / 2);
            _container.anchoredPosition = newPosition;

            // delete any previous settings
            _pagePositions.Clear();

            // iterate through all container childern and set their positions
            for (int i = 0; i < _pageCount; i++)
            {
                RectTransform child = _container.GetChild(i).GetComponent<RectTransform>();
                Vector2 childPosition;
                if (_horizontal)
                {
                    childPosition = new Vector2(i * width - containerWidth / 2 + offsetX, 0f);
                }
                else
                {
                    childPosition = new Vector2(0f, -(i * height - containerHeight / 2 + offsetY));
                }
                child.anchoredPosition = childPosition;
                _pagePositions.Add(-childPosition);
            }
        }

        //------------------------------------------------------------------------
        private void SetPage(int aPageIndex)
        {
            CheckButtonState(aPageIndex);
            aPageIndex = Mathf.Clamp(aPageIndex, 0, _pageCount - 1);
            _container.anchoredPosition = _pagePositions[aPageIndex];
            _currentPage = aPageIndex;
        }

        //------------------------------------------------------------------------
        public void LerpToPage(int aPageIndex)
        {
            CheckButtonState(aPageIndex);
            var index = Mathf.Clamp(aPageIndex, 0, _pageCount - 1);
            aPageIndex = index;
            _lerpTo = _pagePositions[aPageIndex];
            _lerp = true;
            _currentPage = aPageIndex;
            if (pageCountText) pageCountText.text = $"{_currentPage + 1}/{_pageCount}";
            onPageChange?.Invoke(aPageIndex);
        }

        void CheckButtonState(int aPageIndex)
        {
            if (prevButton)
            {
                bool canPrev = aPageIndex > 0;
                prevButton.interactable = canPrev;
                if (hideTransitionButton)
                {
                    nextButton.gameObject.SetActive(prevButton);
                }
            }
            if (nextButton)
            {
                bool canNext = aPageIndex < _pageCount - 1;
                nextButton.interactable = canNext;
                if (hideTransitionButton)
                {
                    nextButton.gameObject.SetActive(canNext);
                }
            }
        }

        //------------------------------------------------------------------------
        private void InitPageSelection(int pageCount)
        {
            int additionPage = pageCount - _pageSelectionImages.Count;
            Debug.Log("Init page selection: " + additionPage);
            for (int i = 0; i < additionPage; i++)
            {
                var obj = Instantiate<Image>(pageSelectionPrefab, pageSelectionIcons);
                _pageSelectionImages.Add(obj);
            }
            for (int i = 0; i < _pageSelectionImages.Count; i++)
            {
                _pageSelectionImages[i].gameObject.SetActive(i < pageCount);
            }
            for (int i = 1; i < _pageSelectionImages.Count; i++)
            {
                Image image = _pageSelectionImages[i];
                image.sprite = unselectedPage;
            }
        }

        //------------------------------------------------------------------------
        private void SetPageSelection(int aPageIndex)
        {
            if (_showPageSelection)
            {
                if (_previousPageSelectionIndex == aPageIndex)
                {
                    return;
                }
                if (_previousPageSelectionIndex >= 0)
                {
                    _pageSelectionImages[_previousPageSelectionIndex].sprite = unselectedPage;
                    _pageSelectionImages[_previousPageSelectionIndex].SetNativeSize();
                }
                _pageSelectionImages[aPageIndex].sprite = selectedPage;
                _pageSelectionImages[aPageIndex].SetNativeSize();
                _previousPageSelectionIndex = aPageIndex;
            }
        }

        //------------------------------------------------------------------------
        private void NextScreen()
        {
            LerpToPage(_currentPage + 1);
        }

        //------------------------------------------------------------------------
        private void PreviousScreen()
        {
            LerpToPage(_currentPage - 1);
        }

        //------------------------------------------------------------------------
        private int GetNearestPage()
        {
            // based on distance from current position, find nearest page
            Vector2 currentPosition = _container.anchoredPosition;

            float distance = float.MaxValue;
            int nearestPage = _currentPage;

            for (int i = 0; i < _pagePositions.Count; i++)
            {
                float testDist = Vector2.SqrMagnitude(currentPosition - _pagePositions[i]);
                if (testDist < distance)
                {
                    distance = testDist;
                    nearestPage = i;
                }
            }

            return nearestPage;
        }

        //------------------------------------------------------------------------
        public void OnBeginDrag(PointerEventData aEventData)
        {
            // if currently lerping, then stop it as user is draging
            _lerp = false;
            // not dragging yet
            _dragging = false;
        }

        //------------------------------------------------------------------------
        public void OnEndDrag(PointerEventData aEventData)
        {
            // how much was container's content dragged
            float difference;
            if (_horizontal)
            {
                difference = _startPosition.x - _container.anchoredPosition.x;
            }
            else
            {
                difference = -(_startPosition.y - _container.anchoredPosition.y);
            }

            // test for fast swipe - swipe that moves only +/-1 item
            if (Time.unscaledTime - _timeStamp < fastSwipeThresholdTime &&
                Mathf.Abs(difference) > fastSwipeThresholdDistance &&
                Mathf.Abs(difference) < _fastSwipeThresholdMaxLimit)
            {
                if (difference > 0)
                {
                    NextScreen();
                }
                else
                {
                    PreviousScreen();
                }
            }
            else
            {
                // if not fast time, look to which page we got to
                LerpToPage(GetNearestPage());
            }

            _dragging = false;
        }

        //------------------------------------------------------------------------
        public void OnDrag(PointerEventData aEventData)
        {
            if (!_dragging)
            {
                // dragging started
                _dragging = true;
                // save time - unscaled so pausing with Time.scale should not affect it
                _timeStamp = Time.unscaledTime;
                // save current position of cointainer
                _startPosition = _container.anchoredPosition;
            }
            else
            {
                if (_showPageSelection)
                {
                    SetPageSelection(GetNearestPage());
                }
            }
        }

    }
}