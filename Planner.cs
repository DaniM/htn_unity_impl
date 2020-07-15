using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Text;

namespace AI
{
    public class PlanDecompositionEntry
    {
        public int ID = 0;
        public ITaskNodeVisitor Visitor = null;
        public int ChildIdx = 0;

        public PlanDecompositionEntry(int id, ITaskNodeVisitor visitor)
        {
            ID = id;
            Visitor = visitor;
        }
    }

    public class PlanState
    {
        public WorldStateDefinition WorldDef;
        public int[] WS;

#if DEBUG

        public int[] Debug_WS;
        public Stack<int[]> DEBUG_WS_STACK = new Stack<int[]>();

#endif

        public Stack<PlanDecompositionEntry> DecompositionHistory = new Stack<PlanDecompositionEntry>();


        public List<int> PlanMTR = new List<int>();
        public List<ITaskNode> Plan = new List<ITaskNode>();
        public List<ITaskNode> MTRNodes = new List<ITaskNode>();
        public bool Found = false;
        public bool Finished = false;

        public int mtrIdx;
        public List<int> mtrReplan;

        public PlanState(WorldStateDefinition wdef, int[] currentWS)
        {
            WorldDef = wdef;
            WS = wdef.CopyWorldState(currentWS);
#if DEBUG

            Debug_WS = wdef.CopyWorldState(currentWS);
#endif
        }
    }

    public class PlannerResult
    {
#if DEBUG

        public int[] Debug_WS;

#endif
        public List<int> PlanMTR = new List<int>();
        public List<ITaskNode> MTRNodes = new List<ITaskNode>();
        public List<ITaskNode> Plan = new List<ITaskNode>();
        public bool Found = false;


        public void Clear()
        {
            PlanMTR.Clear();
            MTRNodes.Clear();
            Plan.Clear();
            Found = false;
        }
    }

    public class PlanExecution<T1, T2>
    {
        public List<int> PlanMTR = new List<int>();
        public List<ITaskNode> Plan = new List<ITaskNode>();
        public int CurrentTaskIdx = 0;
        public TaskStatus LastStatus = TaskStatus.SUCCESS;

        public T1 DataSource;
        public T2 TasksLibrary;

        public ITask CurrentTask;

        //public Dictionary<ITask, List<ITask>> childrenTasks = new Dictionary<ITask, List<ITask>>();

        public List<ITask> childrenTasks = new List<ITask>();

        public List<ICondition> monitors = new List<ICondition>();

        public PlanExecution(T1 dataSource, T2 tasksLibrary)
        {
            DataSource = dataSource;
            TasksLibrary = tasksLibrary;
        }

        public PlanExecution(List<ITaskNode> plan, List<int> mtr, T1 dataSource, T2 tasksLibrary)
        {
            Plan = plan;
            PlanMTR = mtr;
            DataSource = dataSource;
            TasksLibrary = tasksLibrary;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append( '[' );
            for (int i = 0; i < Plan.Count; i++)
            {
                sb.Append(Plan[i].GetName());

                if ( i != Plan.Count -1 )
                {
                    sb.Append(',');
                }
            }
            sb.Append("]\n");
            return sb.ToString();
        }

        public bool HasPlan()
        {
            return Plan.Count > 0;
        }

        public void StopTask(ITask task)
        {
            childrenTasks.Remove(task);
            task.Stop();
        }

        public ITask AddExecutingTask(ITask task)
        {
            childrenTasks.Add(task);
            return task;
        }

        public void ResetPlan()
        {
            for (int i = 0; i < childrenTasks.Count; i++)
            {
                childrenTasks[i].Stop();
            }
            childrenTasks.Clear();
            monitors.Clear();
            CurrentTask = null;
            CurrentTaskIdx = 0;
            LastStatus = TaskStatus.SUCCESS;
        }

        public void Abort()
        {
            for (int i = 0; i < childrenTasks.Count; i++)
            {
                childrenTasks[i].Stop();
            }
            childrenTasks.Clear();
            if (CurrentTask != null)
            {
                CurrentTask.Stop();
                CurrentTask = null;
            }
            monitors.Clear();
            LastStatus = TaskStatus.FAILURE;
        }

        public void Tick( int[] ws )
        {
            switch ( LastStatus )
            {
                case TaskStatus.SUCCESS:
                    while (LastStatus == TaskStatus.SUCCESS)
                    {
                        if (CurrentTask != null)
                        {
                            Plan[CurrentTaskIdx].ApplyEffects(ws);
                            CurrentTaskIdx++;

                            for (int i = 0; i < childrenTasks.Count; i++)
                            {
                                childrenTasks[i].Stop();
                            }

                            childrenTasks.Clear();
                        }

                        if (CurrentTaskIdx < Plan.Count)
                        {
                            if (Plan[CurrentTaskIdx].CheckConditions(ws))
                            {
                                buildActiveMonitorsForTask(Plan[CurrentTaskIdx]);

#if DEBUG
                                try
                                {
#endif

                                    CurrentTask = Plan[CurrentTaskIdx].GetOperator().CreateTask<T1, T2>(DataSource, TasksLibrary);

#if DEBUG
                                }
                                catch (Exception ex)
                                {
                                    Debug.LogErrorFormat( "Task: {0} creation failed. Exception: {1}", Plan[CurrentTaskIdx].GetOperator().GetTaskName(), ex.ToString() );
                                }
#endif

#if DEBUG
                                Assert.IsNotNull( CurrentTask, string.Format( "Node {0} doesn't have a valid operator {1}", Plan[CurrentTaskIdx].GetName(), Plan[CurrentTaskIdx].GetOperator()) );
#endif


                                LastStatus = CurrentTask.Tick();
                                return;
                            }
                            else
                            {
                                LastStatus = TaskStatus.FAILURE;
                            }
                        }
                        else
                        {
                            if ( CurrentTask != null )
                            {
                                for (int i = 0; i < childrenTasks.Count; i++)
                                {
                                    childrenTasks[i].Stop();
                                }
                                childrenTasks.Clear();
                                monitors.Clear();
                            }
                            CurrentTask = null;
                            break;
                        }
                    }
                    break;
                case TaskStatus.CONTINUE:
                    for (int i = 0; i < monitors.Count; i++)
                    {
                        if ( !monitors[i].Eval( ws ) )
                        {
                            Abort();
                            return;
                        }
                    }

                    for (int i = childrenTasks.Count - 1; i >= 0; i--)
                    {
                        if (childrenTasks[i].Tick() != TaskStatus.CONTINUE)
                        {
                            childrenTasks.RemoveAt( i );
                        }
                    }
                    LastStatus = CurrentTask.Tick();
                    break;
                default:
                    break;
            }
        }

        public bool HasFinished()
        {
            return LastStatus == TaskStatus.FAILURE || ( CurrentTaskIdx == Plan.Count && LastStatus == TaskStatus.SUCCESS );
        }

        public void SetPlan(PlannerResult plan)
        {
            Abort();
            Plan.Clear();
            PlanMTR.Clear();

            Plan.AddRange( plan.Plan );
            PlanMTR.AddRange( plan.PlanMTR );

            ResetPlan();
        }

        private void buildActiveMonitorsForTask( ITaskNode taskNode )
        {
            monitors.Clear();
            ICondition monitor = taskNode.GetMonitor();
            if ( monitor != null )
            {
                monitors.Add( monitor );
            }

            ITaskNode parent = taskNode.GetParentTask();
            while ( parent != null )
            {
                monitor = parent.GetMonitor();
                if (monitor != null)
                {
                    monitors.Add(monitor);
                }
                parent = parent.GetParentTask();
            }
        }
    }

    interface IPlanner
    {
        
    }

    /**/
    public class Planner
    {
        public int CompareMTRs(List<int> mtr1, List<int> mtr2)
        {
            int len = mtr1.Count;
            int shortest = 0;
            if (mtr2.Count != len)
            {
                if (mtr2.Count < len)
                {
                    len = mtr2.Count;
                    shortest = 1;
                }
                else
                {
                    shortest = -1;
                }
            }

            for (int i = 0; i < len; i++)
            {
                if ( mtr1[i] != mtr2[i] )
                {
                    if (mtr1[i] < mtr2[i])
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }
            }

            return shortest;
        }

        public PlanState FindPlanIterative(ITaskNode root, WorldStateDefinition worldDef, int[] currentWS, int maxTasksPerIt)
        {
            PlanState pState = new PlanState( worldDef, currentWS );

#if DEBUG
            pState.DEBUG_WS_STACK.Push(worldDef.CopyWorldState(currentWS));
#endif
            ICondition rootCondition = root.GetCondition();
            if (rootCondition != null && !rootCondition.Eval(currentWS))
            {
                pState.Finished = true;
                return pState;
            }

            pState.DecompositionHistory.Push( new PlanDecompositionEntry( 0, root.CreateVisitor() ) );
            Step( pState, maxTasksPerIt );
            return pState;
        }

        public PlanState ReplanIterative(ITaskNode root, WorldStateDefinition worldDef, int[] currentWS, int maxTasksPerIt, List<int> mtr)
        {
            PlanState pState = new PlanState(worldDef, currentWS);
            pState.mtrIdx = 0;
            pState.mtrReplan = mtr;


#if DEBUG
            pState.DEBUG_WS_STACK.Push(worldDef.CopyWorldState(currentWS));
#endif

            ICondition rootCondition = root.GetCondition();
            if (rootCondition != null && !rootCondition.Eval(currentWS))
            {
                pState.Finished = true;
                return pState;
            }

            pState.DecompositionHistory.Push(new PlanDecompositionEntry(0, root.CreateVisitor()));
            ReplanStep(pState, maxTasksPerIt);
            return pState;
        }


        public bool Step( PlanState planState, int maxTasks )
        {
            int tasksExplored = 0;
            if (!planState.Finished)
            {
                while ( planState.DecompositionHistory.Count > 0 )
                {
                    PlanDecompositionEntry entry = planState.DecompositionHistory.Peek();

                    ITaskNodeVisitor visitor = entry.Visitor;

                    if ( visitor.HasNext( ) )
                    {
                        ITaskNode node = visitor.NextChild();
                        ICondition condition = node.GetCondition();
                        if (condition == null || condition.Eval(planState.WS))
                        {
                            if (node.IsComposite())
                            {
#if DEBUG
                                planState.DEBUG_WS_STACK.Push(planState.WorldDef.CopyWorldState(planState.WS));
#endif
                                planState.DecompositionHistory.Push(new PlanDecompositionEntry(entry.ChildIdx, node.CreateVisitor()));
                            }
                            else
                            {
                                planState.Plan.Add(node);
                                visitor.SetVisitResult(true);
                            }

                            node.ApplyEffects(planState.WS);
                            planState.PlanMTR.Add(entry.ChildIdx);
                            planState.MTRNodes.Add(node);
                        }
                        else
                        {
                            visitor.SetVisitResult(false);
                        }


                        entry.ChildIdx++;
                    }
                    else
                    {
#if DEBUG
                        int[] ws = planState.DEBUG_WS_STACK.Pop();
#endif

                        bool visitResult = visitor.GetVisitResult();
                        if ( !visitResult )
                        {
                            // restore
                            while ( visitor.HasPrevious( ) )
                            {
                                ITaskNode undone = visitor.PrevChild();
                                undone.RestoreEffects( planState.WS );
                                planState.Plan.RemoveAt(planState.Plan.Count - 1);
                                planState.PlanMTR.RemoveAt(planState.PlanMTR.Count - 1);
                                planState.MTRNodes.RemoveAt(planState.MTRNodes.Count - 1);
                            }

                            visitor.GetNode().RestoreEffects( planState.WS );

#if DEBUG
                            Assert.IsTrue( planState.WorldDef.IsSameWorldState(ws, planState.WS), "World state restoration failed" );
#endif
                        }

                        

                        planState.DecompositionHistory.Pop();

                        if (planState.DecompositionHistory.Count > 0)
                        {
                            if ( !visitResult )
                            {
                                planState.PlanMTR.RemoveAt(planState.PlanMTR.Count - 1);
                                planState.MTRNodes.RemoveAt(planState.MTRNodes.Count - 1);
                            }

                            planState.DecompositionHistory.Peek().Visitor.SetVisitResult( visitResult );
                        }
                        else
                        {
                            planState.Finished = visitResult;
                            planState.Found = visitResult;
                        }
                    }


                    if ( tasksExplored >= maxTasks )
                    {
                        break;
                    }
                }
                planState.Finished = true;
            }
            
            return planState.Finished;
        }

        public bool ReplanStep(PlanState planState, int maxTasks)
        {
            int tasksExplored = 0;

            if (!planState.Finished)
            {
                while (planState.DecompositionHistory.Count > 0)
                {
                    PlanDecompositionEntry entry = planState.DecompositionHistory.Peek();

                    ITaskNodeVisitor visitor = entry.Visitor;

                    if (visitor.HasNext())
                    {
                        ITaskNode node = visitor.NextChild();
                        ICondition condition = node.GetCondition();

                        if ( CompareMTRs( planState.PlanMTR, planState.mtrReplan ) >= 0 )
                        {
                            planState.Finished = true;
                            planState.Found = false;
                            return planState.Found;
                        }

                        if (condition == null || condition.Eval(planState.WS))
                        {
                            if (node.IsComposite())
                            {
#if DEBUG
                                planState.DEBUG_WS_STACK.Push(planState.WorldDef.CopyWorldState(planState.WS));
#endif
                                planState.DecompositionHistory.Push(new PlanDecompositionEntry(entry.ChildIdx, node.CreateVisitor()));
                            }
                            else
                            {
                                planState.Plan.Add(node);
                                visitor.SetVisitResult(true);
                            }

                            node.ApplyEffects(planState.WS);
                            planState.PlanMTR.Add(entry.ChildIdx);
                            planState.MTRNodes.Add(node);
                            planState.mtrIdx++;
                        }
                        else
                        {
                            visitor.SetVisitResult(false);
                        }


                        entry.ChildIdx++;
                    }
                    else
                    {
#if DEBUG
                        int[] ws = planState.DEBUG_WS_STACK.Pop();
#endif

                        bool visitResult = visitor.GetVisitResult();
                        if (!visitResult)
                        {
                            // restore
                            while (visitor.HasPrevious())
                            {
                                ITaskNode undone = visitor.PrevChild();
                                undone.RestoreEffects(planState.WS);
                                planState.Plan.RemoveAt(planState.Plan.Count - 1);
                                planState.PlanMTR.RemoveAt(planState.PlanMTR.Count - 1);
                                planState.MTRNodes.RemoveAt(planState.MTRNodes.Count - 1);
                                planState.mtrIdx--;
                            }

                            visitor.GetNode().RestoreEffects(planState.WS);

#if DEBUG
                            Assert.IsTrue(planState.WorldDef.IsSameWorldState(ws, planState.WS), "World state restoration failed");
#endif
                        }

                        planState.DecompositionHistory.Pop();

                        if (planState.DecompositionHistory.Count > 0)
                        {
                            if ( !visitResult )
                            {
                                planState.PlanMTR.RemoveAt(planState.PlanMTR.Count - 1);
                                planState.MTRNodes.RemoveAt(planState.MTRNodes.Count - 1);
                                planState.mtrIdx--;
                            }
                            planState.DecompositionHistory.Peek().Visitor.SetVisitResult(visitResult);
                        }
                        else
                        {
                            planState.Found = visitResult;
                        }
                    }


                    if (tasksExplored >= maxTasks)
                    {
                        break;
                    }
                }

                planState.Finished = true;
            }

            return planState.Found;
        }


        public PlannerResult FindPlan( ITaskNode root, WorldStateDefinition worldDef, int[] currentWS)
        {
            PlannerResult result = new PlannerResult();
            ICondition rootCondition = root.GetCondition();
            if (rootCondition != null && !rootCondition.Eval( currentWS ) )
            {
                return result;
            }

            Stack<PlanDecompositionEntry> DecompositionHistory = new Stack<PlanDecompositionEntry>();
            int[] workingWS = worldDef.CopyWorldState(currentWS);


#if DEBUG
            Stack<int[]> DEBUG_WS_STACK = new Stack<int[]>();
            DEBUG_WS_STACK.Push(worldDef.CopyWorldState(workingWS));
#endif
            DecompositionHistory.Push(new PlanDecompositionEntry(0, root.CreateVisitor()));
            while (DecompositionHistory.Count > 0)
            {
                PlanDecompositionEntry entry = DecompositionHistory.Peek();

                ITaskNodeVisitor visitor = entry.Visitor;

                if (visitor.HasNext())
                {
                    ITaskNode node = visitor.NextChild();

                    ICondition condition = node.GetCondition();
                    if (condition == null || condition.Eval(workingWS))
                    {
                        if (node.IsComposite())
                        {
#if DEBUG
                            DEBUG_WS_STACK.Push(worldDef.CopyWorldState(workingWS));
#endif
                            DecompositionHistory.Push(new PlanDecompositionEntry(entry.ChildIdx, node.CreateVisitor()));
                        }
                        else
                        {
                            result.Plan.Add(node);
                            visitor.SetVisitResult(true);
                        }

                        node.ApplyEffects(workingWS);
                        result.PlanMTR.Add(entry.ChildIdx);
                        result.MTRNodes.Add( node );
                    }
                    else
                    {
                        visitor.SetVisitResult(false);
                    }


                    entry.ChildIdx++;
                }
                else
                {
#if DEBUG
                    int[] ws = DEBUG_WS_STACK.Pop();
#endif


                    bool visitResult = visitor.GetVisitResult();
                    if (!visitResult)
                    {
                        // restore
                        while (visitor.HasPrevious())
                        {
                            ITaskNode undone = visitor.PrevChild();
                            undone.RestoreEffects(workingWS);
                            result.Plan.RemoveAt(result.Plan.Count - 1);
                            result.PlanMTR.RemoveAt(result.PlanMTR.Count - 1);
                            result.MTRNodes.RemoveAt(result.MTRNodes.Count - 1);
                        }

                        visitor.GetNode().RestoreEffects(workingWS);

#if DEBUG
                        Assert.IsTrue(worldDef.IsSameWorldState(ws, workingWS), "World state restoration failed");
#endif
                    }

                    DecompositionHistory.Pop();

                    if (DecompositionHistory.Count > 0)
                    {
                        if ( !visitResult )
                        {
                            result.PlanMTR.RemoveAt(result.PlanMTR.Count - 1);
                            result.MTRNodes.RemoveAt(result.MTRNodes.Count - 1);
                        }

                        DecompositionHistory.Peek().Visitor.SetVisitResult(visitResult);
                    }
                    else
                    {
                        result.Found = visitResult;
                    }
                }
            }

            return result;
        }

        public void FindPlan(ITaskNode root, WorldStateDefinition worldDef, int[] currentWS, Stack<PlanDecompositionEntry> DecompositionHistory, PlannerResult result )
        {
            DecompositionHistory.Clear();
            result.Clear();
            ICondition rootCondition = root.GetCondition();
            if (rootCondition != null && !rootCondition.Eval(currentWS))
            {
                return;
            }

            int[] workingWS = worldDef.CopyWorldState(currentWS);


#if DEBUG
            Stack<int[]> DEBUG_WS_STACK = new Stack<int[]>();
            DEBUG_WS_STACK.Push(worldDef.CopyWorldState(workingWS));
#endif
            DecompositionHistory.Push(new PlanDecompositionEntry(0, root.CreateVisitor()));
            while (DecompositionHistory.Count > 0)
            {
                PlanDecompositionEntry entry = DecompositionHistory.Peek();

                ITaskNodeVisitor visitor = entry.Visitor;

                if (visitor.HasNext())
                {
                    ITaskNode node = visitor.NextChild();

                    ICondition condition = node.GetCondition();
                    if (condition == null || condition.Eval(workingWS))
                    {
                        if (node.IsComposite())
                        {
#if DEBUG
                            DEBUG_WS_STACK.Push(worldDef.CopyWorldState(workingWS));
#endif
                            DecompositionHistory.Push(new PlanDecompositionEntry(entry.ChildIdx, node.CreateVisitor()));
                        }
                        else
                        {
                            result.Plan.Add(node);
                            visitor.SetVisitResult(true);
                        }

                        node.ApplyEffects(workingWS);
                        result.PlanMTR.Add(entry.ChildIdx);
                        result.MTRNodes.Add(node);
                    }
                    else
                    {
                        visitor.SetVisitResult(false);
                    }


                    entry.ChildIdx++;
                }
                else
                {
#if DEBUG
                    int[] ws = DEBUG_WS_STACK.Pop();
#endif


                    bool visitResult = visitor.GetVisitResult();
                    if (!visitResult)
                    {
                        // restore
                        while (visitor.HasPrevious())
                        {
                            ITaskNode undone = visitor.PrevChild();
                            undone.RestoreEffects(workingWS);
                            result.Plan.RemoveAt(result.Plan.Count - 1);
                            result.PlanMTR.RemoveAt(result.PlanMTR.Count - 1);
                            result.MTRNodes.RemoveAt(result.MTRNodes.Count - 1);
                        }

                        visitor.GetNode().RestoreEffects(workingWS);

#if DEBUG
                        Assert.IsTrue(worldDef.IsSameWorldState(ws, workingWS), "World state restoration failed");
#endif
                    }

                    DecompositionHistory.Pop();

                    if (DecompositionHistory.Count > 0)
                    {
                        if (!visitResult)
                        {
                            result.PlanMTR.RemoveAt(result.PlanMTR.Count - 1);
                            result.MTRNodes.RemoveAt(result.MTRNodes.Count - 1);
                        }

                        DecompositionHistory.Peek().Visitor.SetVisitResult(visitResult);
                    }
                    else
                    {
                        result.Found = visitResult;
                    }
                }
            }
        }


        public PlannerResult Replan(ITaskNode root, WorldStateDefinition worldDef, int[] currentWS, List<int> mtr)
        {
            PlannerResult result = new PlannerResult();

            ICondition rootCondition = root.GetCondition();
            if (rootCondition != null && !rootCondition.Eval(currentWS))
            {
                return result;
            }

            Stack<PlanDecompositionEntry> DecompositionHistory = new Stack<PlanDecompositionEntry>();
            int[] workingWS = worldDef.CopyWorldState(currentWS);
            int mtrIdx = 0;

#if DEBUG
            Stack<int[]> DEBUG_WS_STACK = new Stack<int[]>();
            DEBUG_WS_STACK.Push(worldDef.CopyWorldState(workingWS));
#endif


            DecompositionHistory.Push(new PlanDecompositionEntry(0, root.CreateVisitor()));
            while (DecompositionHistory.Count > 0)
            {
                PlanDecompositionEntry entry = DecompositionHistory.Peek();

                ITaskNodeVisitor visitor = entry.Visitor;

                if (visitor.HasNext())
                {
                    ITaskNode node = visitor.NextChild();
                    ICondition condition = node.GetCondition();

                    if ( CompareMTRs( result.PlanMTR, mtr ) >= 0 )
                    {
                        result.Found = false;
                        return result;
                    }

                    if (condition == null || condition.Eval(workingWS))
                    {
                        if (node.IsComposite())
                        {
#if DEBUG
                            DEBUG_WS_STACK.Push(worldDef.CopyWorldState(workingWS));
#endif

                            DecompositionHistory.Push(new PlanDecompositionEntry(entry.ChildIdx, node.CreateVisitor()));
                        }
                        else
                        {
                            result.Plan.Add(node);
                            visitor.SetVisitResult(true);
                        }

                        node.ApplyEffects(workingWS);
                        result.PlanMTR.Add(entry.ChildIdx);
                        result.MTRNodes.Add( node );
                        mtrIdx++;
                    }
                    else
                    {
                        visitor.SetVisitResult(false);
                    }


                    entry.ChildIdx++;
                }
                else
                {
#if DEBUG
                    int[] ws = DEBUG_WS_STACK.Pop();
#endif
                    bool visitResult = visitor.GetVisitResult();
                    if (!visitResult)
                    {
                        // restore
                        while (visitor.HasPrevious())
                        {
                            ITaskNode undone = visitor.PrevChild();
                            undone.RestoreEffects(workingWS);
                            result.Plan.RemoveAt(result.Plan.Count - 1);
                            result.PlanMTR.RemoveAt(result.PlanMTR.Count - 1);
                            result.MTRNodes.RemoveAt(result.MTRNodes.Count - 1);
                            mtrIdx--;
                        }

                        visitor.GetNode().RestoreEffects(workingWS);

#if DEBUG
                        Assert.IsTrue(worldDef.IsSameWorldState(ws, workingWS), "World state restoration failed");
#endif

                    }

                    DecompositionHistory.Pop();


                    if (DecompositionHistory.Count > 0)
                    {
                        if (!visitResult)
                        {
                            result.PlanMTR.RemoveAt(result.PlanMTR.Count - 1);
                            result.MTRNodes.RemoveAt(result.MTRNodes.Count - 1);
                            mtrIdx--;
                        }

                        DecompositionHistory.Peek().Visitor.SetVisitResult(visitResult);
                    }
                    else
                    {
                        result.Found = visitResult;
                    }
                }
            }

            return result;
        }

        public void Replan(ITaskNode root, WorldStateDefinition worldDef, int[] currentWS, List<int> mtr, Stack<PlanDecompositionEntry> DecompositionHistory, PlannerResult result)
        {
            DecompositionHistory.Clear();
            result.Clear();

            ICondition rootCondition = root.GetCondition();
            if (rootCondition != null && !rootCondition.Eval(currentWS))
            {
                return;
            }

            int[] workingWS = worldDef.CopyWorldState(currentWS);
            int mtrIdx = 0;

#if DEBUG
            Stack<int[]> DEBUG_WS_STACK = new Stack<int[]>();
            DEBUG_WS_STACK.Push(worldDef.CopyWorldState(workingWS));
#endif


            DecompositionHistory.Push(new PlanDecompositionEntry(0, root.CreateVisitor()));
            while (DecompositionHistory.Count > 0)
            {
                PlanDecompositionEntry entry = DecompositionHistory.Peek();

                ITaskNodeVisitor visitor = entry.Visitor;

                if (visitor.HasNext())
                {
                    ITaskNode node = visitor.NextChild();
                    ICondition condition = node.GetCondition();

                    if (CompareMTRs(result.PlanMTR, mtr) >= 0)
                    {
                        result.Found = false;
                        return;
                    }

                    if (condition == null || condition.Eval(workingWS))
                    {
                        if (node.IsComposite())
                        {
#if DEBUG
                            DEBUG_WS_STACK.Push(worldDef.CopyWorldState(workingWS));
#endif

                            DecompositionHistory.Push(new PlanDecompositionEntry(entry.ChildIdx, node.CreateVisitor()));
                        }
                        else
                        {
                            result.Plan.Add(node);
                            visitor.SetVisitResult(true);
                        }

                        node.ApplyEffects(workingWS);
                        result.PlanMTR.Add(entry.ChildIdx);
                        result.MTRNodes.Add(node);
                        mtrIdx++;
                    }
                    else
                    {
                        visitor.SetVisitResult(false);
                    }


                    entry.ChildIdx++;
                }
                else
                {
#if DEBUG
                    int[] ws = DEBUG_WS_STACK.Pop();
#endif
                    bool visitResult = visitor.GetVisitResult();
                    if (!visitResult)
                    {
                        // restore
                        while (visitor.HasPrevious())
                        {
                            ITaskNode undone = visitor.PrevChild();
                            undone.RestoreEffects(workingWS);
                            result.Plan.RemoveAt(result.Plan.Count - 1);
                            result.PlanMTR.RemoveAt(result.PlanMTR.Count - 1);
                            result.MTRNodes.RemoveAt(result.MTRNodes.Count - 1);
                            mtrIdx--;
                        }

                        visitor.GetNode().RestoreEffects(workingWS);

#if DEBUG
                        Assert.IsTrue(worldDef.IsSameWorldState(ws, workingWS), "World state restoration failed");
#endif

                    }

                    DecompositionHistory.Pop();


                    if (DecompositionHistory.Count > 0)
                    {
                        if (!visitResult)
                        {
                            result.PlanMTR.RemoveAt(result.PlanMTR.Count - 1);
                            result.MTRNodes.RemoveAt(result.MTRNodes.Count - 1);
                            mtrIdx--;
                        }

                        DecompositionHistory.Peek().Visitor.SetVisitResult(visitResult);
                    }
                    else
                    {
                        result.Found = visitResult;
                    }
                }
            }
        }
    }
}
