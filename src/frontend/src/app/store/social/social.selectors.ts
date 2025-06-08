import { createFeatureSelector, createSelector } from '@ngrx/store';
import { SocialState } from './social.reducer';

export const selectSocialState = createFeatureSelector<SocialState>('social');

// Activity Feed Selectors
export const selectActivityFeed = createSelector(
  selectSocialState,
  state => state.activityFeed.posts
);

export const selectHasMorePosts = createSelector(
  selectSocialState,
  state => state.activityFeed.hasMore
);

export const selectTotalPostsCount = createSelector(
  selectSocialState,
  state => state.activityFeed.totalCount
);

export const selectFeedLoaded = createSelector(
  selectSocialState,
  state => state.activityFeed.feedLoaded
);

// Selected Post Selectors
export const selectSelectedPost = createSelector(
  selectSocialState,
  state => state.selectedPost
);

// Post Comments Selectors
export const selectPostComments = (postId: number) => createSelector(
  selectSocialState,
  state => state.postComments[postId]?.comments || []
);

export const selectPostCommentsLoaded = (postId: number) => createSelector(
  selectSocialState,
  state => state.postComments[postId]?.loaded || false
);

export const selectPostCommentsHasMore = (postId: number) => createSelector(
  selectSocialState,
  state => state.postComments[postId]?.hasMore || false
);

// Loading and Error Selectors
export const selectSocialLoading = createSelector(
  selectSocialState,
  state => state.isLoading
);

export const selectSocialError = createSelector(
  selectSocialState,
  state => state.error
);
