import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { map, mergeMap, catchError, withLatestFrom } from 'rxjs/operators';
import * as SocialActions from './social.actions';
import { Store } from '@ngrx/store';
import { AppState } from '../app.state';

@Injectable()
export class SocialEffects {
  loadActivityFeed$ = createEffect(() =>
    this.actions$.pipe(
      ofType(SocialActions.loadActivityFeed),
      mergeMap(action =>
        // TODO: Replace with actual service call
        of({
          posts: [],
          hasMore: false,
          totalCount: 0
        }).pipe(
          map(response => SocialActions.loadActivityFeedSuccess({
            posts: response.posts,
            hasMore: response.hasMore,
            totalCount: response.totalCount
          })),
          catchError(error => of(SocialActions.loadActivityFeedFailure({ error })))
        )
      )
    )
  );

  createPost$ = createEffect(() =>
    this.actions$.pipe(
      ofType(SocialActions.createPost),
      mergeMap(action =>
        // TODO: Replace with actual service call
        of({ post: { ...action.postData, id: Date.now() } }).pipe(
          map(response => SocialActions.createPostSuccess({ post: response.post })),
          catchError(error => of(SocialActions.createPostFailure({ error })))
        )
      )
    )
  );

  deletePost$ = createEffect(() =>
    this.actions$.pipe(
      ofType(SocialActions.deletePost),
      mergeMap(action =>
        // TODO: Replace with actual service call
        of({ postId: action.postId }).pipe(
          map(() => SocialActions.deletePostSuccess({ postId: action.postId })),
          catchError(error => of(SocialActions.deletePostFailure({ error })))
        )
      )
    )
  );

  likePost$ = createEffect(() =>
    this.actions$.pipe(
      ofType(SocialActions.likePost),
      mergeMap(action =>
        // TODO: Replace with actual service call
        of({ postId: action.postId, likesCount: 1 }).pipe(
          map(response => SocialActions.likePostSuccess({
            postId: response.postId,
            likesCount: response.likesCount
          })),
          catchError(error => of(SocialActions.likePostFailure({ error })))
        )
      )
    )
  );

  unlikePost$ = createEffect(() =>
    this.actions$.pipe(
      ofType(SocialActions.unlikePost),
      mergeMap(action =>
        // TODO: Replace with actual service call
        of({ postId: action.postId, likesCount: 0 }).pipe(
          map(response => SocialActions.unlikePostSuccess({
            postId: response.postId,
            likesCount: response.likesCount
          })),
          catchError(error => of(SocialActions.unlikePostFailure({ error })))
        )
      )
    )
  );

  loadPostComments$ = createEffect(() =>
    this.actions$.pipe(
      ofType(SocialActions.loadPostComments),
      mergeMap(action =>
        // TODO: Replace with actual service call
        of({
          postId: action.postId,
          comments: [],
          hasMore: false,
          totalCount: 0
        }).pipe(
          map(response => SocialActions.loadPostCommentsSuccess({
            postId: response.postId,
            comments: response.comments,
            hasMore: response.hasMore,
            totalCount: response.totalCount
          })),
          catchError(error => of(SocialActions.loadPostCommentsFailure({ error })))
        )
      )
    )
  );

  addComment$ = createEffect(() =>
    this.actions$.pipe(
      ofType(SocialActions.addComment),
      mergeMap(action =>
        // TODO: Replace with actual service call
        of({
          postId: action.postId,          comment: {
            id: Date.now(),
            content: action.content,
            ...(action.parentCommentId ? { parentCommentId: action.parentCommentId } : {}),
            userId: 1, // TODO: Get from auth state
            user: {
              id: 1,
              firstName: 'Test',
              lastName: 'User'
            },
            likes: 0,
            isLiked: false,
            createdAt: new Date(),
            replies: 0
          }
        }).pipe(
          map(response => SocialActions.addCommentSuccess({
            postId: response.postId,
            comment: response.comment
          })),
          catchError(error => of(SocialActions.addCommentFailure({ error })))
        )
      )
    )
  );
  constructor(
    private actions$: Actions,
    private store: Store<AppState>
  ) {}
}
